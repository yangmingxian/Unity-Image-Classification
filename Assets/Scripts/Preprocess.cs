using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Preprocess : MonoBehaviour
{

    RenderTexture renderTexture;
    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;
    UnityAction<byte[]> callback;

    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback)
    {
        this.callback = callback;
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize, desiredSize, 0, RenderTextureFormat.ARGB32);
        }
        // Vector2 offset = new Vector2((webCamTexture.width - w) / 2, (webCamTexture.width - h) / 2);
        // texture2D.SetPixels(webCamTexture.GetPixels((int)offset.x, (int)offset.y, w, h));
        // texture2D.Apply();
        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        // Graphics.Blit(texture2D, renderTexture, scale, offset);
        Graphics.Blit(webCamTexture, renderTexture, scale, offset);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    Texture2D texture2D;
    public int w = 224, h = 224;
    public byte[] CropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback)
    {
        if (texture2D == null)
        {
            texture2D = new Texture2D(desiredSize, desiredSize, TextureFormat.ARGB32, 0, true);
        }
        Vector2 offset = new Vector2((webCamTexture.width - w) / 2, (webCamTexture.width - h) / 2);
        texture2D.SetPixels(webCamTexture.GetPixels((int)offset.x, (int)offset.y, w, h));
        // texture2D.Apply();

        AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(texture2D, 0, TextureFormat.RGB24, OnCompleteReadback);
        return request.GetData<byte>().ToArray();
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
            return;
        }
        callback.Invoke(request.GetData<byte>().ToArray());
    }

}