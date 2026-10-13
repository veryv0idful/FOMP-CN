using System.IO;
using System.Reflection;
using UnityEngine;

public static class ResourceUtil
{
    public static byte[] GetEmbeddedWavBytes(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Note: Assembly resource names are formatted as: <DefaultNamespace>.<FolderPath>.<FileName>
        // e.g., "MyMelonMod.Resources.your_audio.wav"
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
#if MELON
            MelonLoader.MelonLogger.Error($"Resource '{resourceName}' not found!");
#endif
            return null;
        }

        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        return buffer;
    }
    public static AudioClip WavToAudioClip(byte[] wavBytes, string clipName = "EmbeddedAudio")
    {
        if (wavBytes == null || wavBytes.Length < 44) return null;

        int channels = System.BitConverter.ToInt16(wavBytes, 22);
        int sampleRate = System.BitConverter.ToInt32(wavBytes, 24);
        int bitDepth = System.BitConverter.ToInt16(wavBytes, 34);

        int pos = 12;
        while (pos < wavBytes.Length - 8)
        {
            string chunkId = System.Text.Encoding.ASCII.GetString(wavBytes, pos, 4);
            int chunkSize = System.BitConverter.ToInt32(wavBytes, pos + 4);
            if (chunkId == "data")
            {
                pos += 8;
                break;
            }
            pos += 8 + chunkSize;
        }

        int pcmDataLength = wavBytes.Length - pos;
        int bytesPerSample = bitDepth / 8;
        int totalSamples = pcmDataLength / bytesPerSample;
        int frameCount = totalSamples / channels;

        float[] floatData = new float[totalSamples];

        if (bitDepth == 16)
        {
            for (int i = 0; i < totalSamples; i++)
            {
                short sample = System.BitConverter.ToInt16(wavBytes, pos + (i * 2));
                floatData[i] = sample / 32768f;
            }
        }
        else if (bitDepth == 8)
        {
            for (int i = 0; i < totalSamples; i++)
            {
                byte sample = wavBytes[pos + i];
                floatData[i] = (sample - 128) / 128f;
            }
        }

        AudioClip audioClip = AudioClip.Create(clipName, frameCount, channels, sampleRate, false);
        audioClip.SetData(floatData, 0);

        return audioClip;
    }

    public static AudioClip GetEmbeddedAudioClip(string resourceName, string clipName = "EmbeddedAudio")
    {
        return WavToAudioClip(GetEmbeddedWavBytes(resourceName), clipName);
    }

    public static void PlayAudioClip(AudioClip clip)
    {
    if (clip == null) return;

    Vector3 listenerPos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
    AudioSource.PlayClipAtPoint(clip, listenerPos, 1.0f);
    }
}