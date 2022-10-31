using UnityEngine;
using UnityEngine.UI;

public class CameraView : MonoBehaviour
{

    RawImage rawImage;
    AspectRatioFitter fitter;
    WebCamTexture webcamTexture;

    [SerializeField] Image frame;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        fitter = GetComponent<AspectRatioFitter>();
        InitWebCam();
        // frame.rectTransform.sizeDelta = new Vector2(webcamTexture.height, webcamTexture.height);
        if (webcamTexture.width > 100)
        {
            fitter.aspectRatio = (float)webcamTexture.width / (float)webcamTexture.height;
        }
    }

    void InitWebCam()
    {
        string camName = WebCamTexture.devices[0].name;
        webcamTexture = new WebCamTexture(camName, Screen.width, Screen.height, 30);
        // rawImage.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    public WebCamTexture GetCamImage()
    {
        return webcamTexture;
    }
}