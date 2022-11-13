using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RenderTexture _renderTexture;
    public bool recording;



    public IEnumerator Capture()
    {
        string filename = "Test-";
        string directory = $"{Application.persistentDataPath}/- [ Data ] -";
        
        int captureFrameIndex = 0;
        while (recording)
        { 
            yield return new WaitForEndOfFrame();
            AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.ARGB32, readbackRequest =>
            {
                Texture2D texture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false, true);
                texture.LoadRawTextureData (readbackRequest.GetData<uint>());

                byte[] rawTextureData = texture.GetRawTextureData();
                GraphicsFormat format = texture.graphicsFormat;
                uint width = (uint)texture.width;
                uint height = (uint)texture.height;
                
                Destroy(texture);
                
                new Thread(() =>
                {
                    byte[] bytes = ImageConversion.EncodeArrayToPNG(rawTextureData, format, width, height);

                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                    File.WriteAllBytes($"{directory}/{filename}{captureFrameIndex}.png", bytes);
                }).Start();
            });

            while (!(request.done || request.hasError))
            {
                Debug.Log($"Waiting: {captureFrameIndex}");
                yield return new WaitForEndOfFrame();
            }
            
            Debug.Log($"Saved Image: {captureFrameIndex}");
            captureFrameIndex++;
        }
    }
}
