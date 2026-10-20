using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomNight.Util;


public static class ResourceUtil
{
    public static byte[] GetEmbeddedFileBytes(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) return null;

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

    public static AudioClip GetEmbeddedAudioClip(string resourceName, string clipName = "EmbeddedAudio") => WavToAudioClip(GetEmbeddedFileBytes(resourceName), clipName);
    
    

    public static void PlayAudioClip(this AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, new Vector3(0, 1, -10));
    }
    

public static Texture2D LoadEmbeddedTexture(string resourceName)
{
    byte[] png = GetEmbeddedFileBytes(resourceName);
    if (png == null) return null;

    byte[] rgba = DecodePng(png, out int w, out int h);
    if (rgba == null) return null;

    Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
    texture.hideFlags = HideFlags.HideAndDontSave;

    GCHandle pin = GCHandle.Alloc(rgba, GCHandleType.Pinned);
    try { texture.LoadRawTextureData(pin.AddrOfPinnedObject(), rgba.Length); }
    finally { pin.Free(); }

    texture.Apply(false, false);
    return texture;
}

static byte[] DecodePng(byte[] png, out int w, out int h)
{
    w = h = 0;
    int colorType = -1, bitDepth = 0, interlace = 0;
    MemoryStream idat = new MemoryStream();

    int pos = 8;
    while (pos + 8 <= png.Length)
    {
        int len = (png[pos] << 24) | (png[pos + 1] << 16) | (png[pos + 2] << 8) | png[pos + 3];
        string type = Encoding.ASCII.GetString(png, pos + 4, 4);
        int data = pos + 8;

        if (type == "IHDR")
        {
            w = (png[data] << 24) | (png[data + 1] << 16) | (png[data + 2] << 8) | png[data + 3];
            h = (png[data + 4] << 24) | (png[data + 5] << 16) | (png[data + 6] << 8) | png[data + 7];
            bitDepth = png[data + 8];
            colorType = png[data + 9];
            interlace = png[data + 12];
        }
        else if (type == "IDAT") idat.Write(png, data, len);
        else if (type == "IEND") break;

        pos = data + len + 4;
    }

    if (bitDepth != 8 || interlace != 0 || (colorType != 2 && colorType != 6)) return null;

    int ch = colorType == 6 ? 4 : 3;
    int stride = w * ch;

    idat.Position = 2;
    byte[] raw = new byte[(stride + 1) * h];
    using (DeflateStream ds = new DeflateStream(idat, CompressionMode.Decompress))
    {
        int read = 0;
        while (read < raw.Length)
        {
            int n = ds.Read(raw, read, raw.Length - read);
            if (n <= 0) break;
            read += n;
        }
        if (read < raw.Length) return null;
    }
    
    byte[] rec = new byte[stride * h];
    for (int y = 0; y < h; y++)
    {
        int filter = raw[y * (stride + 1)];
        int src = y * (stride + 1) + 1;
        int cur = y * stride;
        int prev = (y - 1) * stride;

        for (int i = 0; i < stride; i++)
        {
            int x = raw[src + i];
            int a = i >= ch ? rec[cur + i - ch] : 0;
            int b = y > 0 ? rec[prev + i] : 0;
            int c = (y > 0 && i >= ch) ? rec[prev + i - ch] : 0;

            switch (filter)
            {
                case 1: x += a; break;
                case 2: x += b; break;
                case 3: x += (a + b) >> 1; break;
                case 4:
                    int p = a + b - c;
                    int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
                    x += (pa <= pb && pa <= pc) ? a : (pb <= pc ? b : c);
                    break;
            }
            rec[cur + i] = (byte)x;
        }
    }
    
    byte[] outPix = new byte[w * h * 4];
    for (int y = 0; y < h; y++)
    {
        int s = y * stride;
        int d = (h - 1 - y) * w * 4;
        for (int x = 0; x < w; x++)
        {
            outPix[d + x * 4]     = rec[s + x * ch];
            outPix[d + x * 4 + 1] = rec[s + x * ch + 1];
            outPix[d + x * 4 + 2] = rec[s + x * ch + 2];
            outPix[d + x * 4 + 3] = ch == 4 ? rec[s + x * ch + 3] : (byte)255;
        }
    }
    return outPix;
}

    public static Sprite LoadEmbeddedSprite(string resourceName)
    {
        Texture2D texture = LoadEmbeddedTexture(resourceName);
        if (texture == null) return null;

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        texture.hideFlags = HideFlags.HideAndDontSave;
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }
}