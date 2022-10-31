using UnityEngine;
using UnityEngine.UI;

public class CameraView : MonoBehaviour
{
    RawImage rawImage;
    AspectRatioFitter fitter;
    WebCamTexture webcamTexture;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        fitter = GetComponent<AspectRatioFitter>();
        InitWebCam();
        if (webcamTexture.width > 100)
        {
            fitter.aspectRatio = (float)webcamTexture.width / (float)webcamTexture.height;
        }
    }

    void InitWebCam()
    {
        string camName = WebCamTexture.devices[0].name;
        webcamTexture = new WebCamTexture(camName, Screen.width, Screen.height, 30);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    public WebCamTexture GetCamImage()
    {
        return webcamTexture;
    }

}