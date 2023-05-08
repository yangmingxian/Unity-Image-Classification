using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;

public class Classification : MonoBehaviour
{

    const int IMAGE_SIZE = 244;
    const string INPUT_NAME = "images";
    const string OUTPUT_NAME = "Softmax";
    public NNModel modelFile;
    public TextAsset labelAsset;
    public CameraView CameraView;
    public Preprocess preprocess;
    public TMP_Text resText;
    // public TMP_Text resProbText;


    string[] labels;
    IWorker worker;

    void Start()
    {
        var model = ModelLoader.Load(modelFile);
        // worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);

        // 新3.0版直接调用WorkerFactory.Device.GPU即可 2.0需要选择一个合适GPU的模式
        worker = WorkerFactory.CreateWorker(model, WorkerFactory.Device.GPU);

        // 得到 index 对应的 label
        // 根据 " 分割标签，奇数表示 label, 偶数表示 index
        var stringArray = labelAsset.text.Split('"').Where((item, index) => index % 2 != 0);
        labels = stringArray.Where((x, i) => i % 2 != 0).ToArray();
    }

    public void RunClass()
    {
        // 获取当前帧的图像
        WebCamTexture webCamTexture = CameraView.GetCamImage();

        if (webCamTexture.width > 100)
        {
            // 如果有图像的话（其实就是图像不是很小的时候）
            // ScaleAndCropImage 执行完之后 直接callback RunModel
            preprocess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE,
            RunModel);

        }
    }

    void RunModel(byte[] pixels)
    {
        // 最好还是在 StartCoroutine 之前停掉所有的协程
        StopAllCoroutines();
        StartCoroutine(RunModelRoutine(pixels));
    }



    IEnumerator RunModelRoutine(byte[] pixels)
    {
        // 这里的 pixels 经过了 TransformInput 归一化
        Tensor tensor = TransformInput(pixels);
        var inputs = new Dictionary<string, Tensor> {
            { INPUT_NAME, tensor }
        };
        worker.Execute(inputs);

        // PeekOutput 会导致 GarbageCollector 溢出 ，这里使用了官方推荐的 CopyOutput 
        // 或者使用下面注释的这两行 取代 CopyOutput()
        // Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);
        // outputTensor.TakeOwnership();
        Tensor outputTensor = worker.CopyOutput(OUTPUT_NAME);

        List<float> temp = outputTensor.ToReadOnlyArray().ToList();

        // resProbText.text = "";

        // for (int i = 0; i < temp.Count; i++)
        // {
        //     Debug.Log(labels[i] + ":" + temp[i]);
        //     resProbText.text = resProbText.text + $"{labels[i]}: {temp[i].ToString("f2")} \n";
        // }


        float maxP = temp.Max();
        int indexR = temp.IndexOf(maxP);

        resText.text = labels[indexR];

        // 这里 Barracuda 不会自动释放GC，需要手动 Dispose
        // 这里的 worker 也需要 Dispose，不过程序开的时候 worker 需要一直用，所以没有 Dispose
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }

    // 这个函数就是归一化
    Tensor TransformInput(byte[] pixels)
    {
        float[] transformedPixels = new float[pixels.Length];
        // print(pixels.Length);  196608 256*256*3
        for (int i = 0; i < pixels.Length; i++)
        {
            // 这里是 ImageNet 的预训练模型的参数，这里并没有使用相应的模型，
            // 所以注释掉掉了换用了简单的均值归一化

            // if (i % 3 == 0)
            //     transformedPixels[i] = ((pixels[i] / 255f) - 0.485f) / 0.229f;
            // else if (i % 3 == 1)
            //     transformedPixels[i] = ((pixels[i] / 255f) - 0.456f) / 0.224f;
            // else
            //     transformedPixels[i] = ((pixels[i] / 255f) - 0.406f) / 0.225f;

            transformedPixels[i] = (pixels[i] - 127f) / 128f;
        }
        return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels);
    }

    // public static Dictionary<string, double> sortDictByValue(Dictionary<string, double> dic)
    // {
    //     List<KeyValuePair<string, double>> myList = new List<KeyValuePair<string, double>>(dic);
    //     myList.Sort(delegate(KeyValuePair<string, double> s1, KeyValuePair<string, double> s2)
    //     {
    //         return s1.Value.CompareTo(s2.Value);
    //     });
    //     dic.Clear();
    //     foreach (KeyValuePair<string, double> pair in myList)
    //     {
    //          dic.Add(pair.Key, pair.Value);
    //     }
    //     return dic;
    // }

}
