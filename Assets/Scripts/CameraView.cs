using UnityEngine;
using UnityEngine.UI;

public class CameraView : MonoBehaviour
{
    RawImage rawImage;
    AspectRatioFitter fitter;
    public WebCamTexture webcamTexture;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        fitter = GetComponent<AspectRatioFitter>();
        InitWebCam();
        // 这里必须设置一下aspectRatio，要不然图像的放缩和旋转会导致奇怪的走形
        fitter.aspectRatio = (float)webcamTexture.width / (float)webcamTexture.height;
    }

    void InitWebCam()
    {
        string camName = WebCamTexture.devices[0].name;
        // 这里使用的是 设备 屏幕的分辨率 ，场景中居中即可视为裁剪
        //（Script: Preprocess） 在图像预处理的时候 计算了scale和offset来裁剪和缩放原图像到正方形 然后使用Bliz进行分辨率放缩到256
        webcamTexture = new WebCamTexture(camName, Screen.width, Screen.height, 30);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    // 用来捕获相机的图像，返回给 RunClass()
    public WebCamTexture GetCamImage()
    {
        return webcamTexture;
    }

}