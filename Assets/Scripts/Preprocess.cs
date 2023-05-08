using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class Preprocess : MonoBehaviour
{
    RenderTexture renderTexture;
    RenderTexture renderTexture2;
    [SerializeField] RawImage preview;

    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;
    UnityAction<byte[]> callback;

    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback)
    {
        this.callback = callback;
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize, desiredSize, 0, RenderTextureFormat.ARGB32);
            renderTexture2 = new RenderTexture(desiredSize, desiredSize, 0, RenderTextureFormat.ARGB32);
        }
        // 这里做的是 根据屏幕长宽比放缩长方形的图像，这样就等于是舍弃了高的多出来的边缘部分
        // 然后做一个 offset 偏移  然后 Blit 到 renderTexture 上 这样就是把正方形的图像 放缩到了 desiredSize*desiredSize
        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(webCamTexture, renderTexture, scale, offset);

        RenderTexture.GetTemporary(1024, 1024, 16, RenderTextureFormat.Default);
        Graphics.Blit(webCamTexture, renderTexture2, scale, offset);
        // 创建一个preview用来展示所拍照片
        preview.texture = renderTexture2;
        // Graphics.ConvertTexture(webCamTexture, texture2D);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
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