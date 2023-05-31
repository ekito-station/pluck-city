using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeManager : MonoBehaviour
{
    public GameObject arCamera;
    public GameObject markerPrefab;
    public GameObject guitarStrPrefab;
    public GameObject sitarStrPrefab;

    public float updatePitchInterval = 0.1f; // strPitchの値を更新する間隔
    private WaitForSeconds waitToUpdate;

    private int count;
    private Vector3 prePos;

    private bool isGuitar = true;
    public float strRadius = 0.5f;

    private float[] harmonies = { 0.36f, 0.44f, 0.56f, 0.64f, 1.0f, 1.44f, 1.78f, 2.25f, 2.56f, 3.16f, 100.0f };

    public GameObject expText;
    public GameObject pitchTextObj;
    public TextMeshProUGUI pitchText;
    public float front;

    private float sqrInitLength;
    private float strPitch;

    public Slider lengthSlider;
    public TextMeshProUGUI sliderValueText;

    public AudioSource audioSource;
    public AudioClip clickButton;
    public AudioClip clickSound;

    public GameObject guitarButton;
    public GameObject sitarOffButton;
    public GameObject guitarOffButton;
    public GameObject sitarButton;

    public GameObject settingCanvas;
    public GameObject helpCanvas;
    public Scrollbar helpScrollBar;

    void Start()
    {
        // 初期化
        waitToUpdate = new WaitForSeconds(updatePitchInterval);
        count = 0;
        // スライダーの初期化
        float initLength = lengthSlider.value;
        sqrInitLength = initLength * initLength;
        sliderValueText.text = Mathf.Floor(initLength * 100).ToString() + "cm";
        Debug.Log("DebugLog sqrInitLength: " + sqrInitLength);
        // strPitchの値を更新するコルーチンを開始
        StartCoroutine(UpdatePitch());
        Debug.Log("Started UpdatePitch Coroutine.");
    }

    private IEnumerator UpdatePitch()
    {
        while (true)
        {
            Transform camTran = arCamera.transform;
            Vector3 curPos = camTran.position + front * camTran.forward;
            float sqrDist = (curPos - prePos).sqrMagnitude;

            float ratio = sqrDist / sqrInitLength;
            foreach (float harmony in harmonies)
            {
                if (ratio < harmony)
                {
                    // strPitchの値を更新
                    strPitch = harmony;
                    switch (strPitch)
                    {
                        case 0.36f:
                            pitchText.text = "B3";
                            break;
                        case 0.44f:
                            pitchText.text = "A3";
                            break;
                        case 0.56f:
                            pitchText.text = "G3";
                            break;
                        case 0.64f:
                            pitchText.text = "F3";
                            break;
                        case 1.0f:
                            pitchText.text = "E3";
                            break;
                        case 1.44f:
                            pitchText.text = "C3";
                            break;
                        case 1.78f:
                            pitchText.text = "A2";
                            break;
                        case 2.25f:
                            pitchText.text = "G2";
                            break;
                        case 2.56f:
                            pitchText.text = "F2";
                            break;
                        case 3.16f:
                            pitchText.text = "E2";
                            break;
                        case 100.0f:
                            pitchText.text = "D2";
                            break;
                        default:
                            pitchText.text = "";
                            break;
                    }
                    break;
                }
            }
            if (count > 0)
            {
                expText.SetActive(true);
                pitchTextObj.SetActive(true);
            }
            yield return waitToUpdate;
        }
    }

    public void OnLengthSliderValueChanged()
    {
        float initLength = lengthSlider.value;
        sqrInitLength = initLength * initLength;
        Debug.Log("DebugLog sqrInitLength: " + sqrInitLength);

        sliderValueText.text = Mathf.Floor(initLength * 100).ToString() + "cm";
    }

    public void OnClickMarkerButton()
    {
        Debug.Log("Clicked Marker Button.");
        if (count == 0) audioSource.PlayOneShot(clickButton);

        // ピンを配置
        Transform camTran = arCamera.transform;
        Vector3 curPos = camTran.position + front * camTran.forward;
        GameObject marker =  Instantiate(markerPrefab, curPos, Quaternion.identity);
        marker.transform.LookAt(camTran);   // カメラの方を向かせる        
        Debug.Log("Placed a marker.");

        // 2つ目以降のピンなら弦を張る
        if (count > 0)
        {
            // curPosは置いたばかりのピンの座標でprePosは1つ前のピンの座標
            Vector3 strVec = curPos - prePos;           // 弦の方向を取得
            float dist = strVec.magnitude;              // 弦の長さを取得
            Vector3 strY = new Vector3(0f, dist, 0f);
            Vector3 halfStrVec = strVec * 0.5f;
            Vector3 centerCoord = prePos + halfStrVec;  // 弦の中点の座標        
            GameObject str = isGuitar ? Instantiate(guitarStrPrefab, centerCoord, Quaternion.identity) : Instantiate(sitarStrPrefab, centerCoord, Quaternion.identity);
            str.transform.localScale = new Vector3(strRadius, dist / 2, strRadius); // ひとまず弦をY軸方向に伸ばす

            // CapsuleCollider col = str.GetComponent<CapsuleCollider>();
            // col.isTrigger = true;   // 衝突判定を行わないように
            str.transform.rotation = Quaternion.FromToRotation(strY, strVec);   // 弦を本来の方向に回転

            // 弦にプロパティ(pitch)を追加
            StringController stringController = str.GetComponent<StringController>();
            stringController.pitch = strPitch;
        }
        // ピンの座標を保存
        prePos = curPos;
        count += 1;
        Debug.Log("count: " + count.ToString());
    }

    public void OnClickTrashButton()
    {
        Debug.Log("Clicked Trash Button.");
        audioSource.PlayOneShot(clickButton);

        expText.SetActive(false);
        pitchText.text = "";
        pitchTextObj.SetActive(false);        
        count = 0;
        float initLength = lengthSlider.value;
        sqrInitLength = initLength * initLength;

        GameObject[] markers = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject marker in markers)
        {
            Destroy(marker);
        }
        Debug.Log("Deleted all my markers.");

        GameObject[] guitarStrs = GameObject.FindGameObjectsWithTag("Guitar");
        foreach (GameObject guitarStr in guitarStrs)
        {
            Destroy(guitarStr);
        }
        GameObject[] sitarStrs = GameObject.FindGameObjectsWithTag("Sitar");
        foreach (GameObject sitarStr in sitarStrs)
        {
            Destroy(sitarStr);
        }        
        Debug.Log("Deleted all strings.");
    }

    public void OnClickSettingButton()
    {
        audioSource.PlayOneShot(clickButton);
        settingCanvas.SetActive(true);
    }

    public void OnClickHelpButton()
    {
        audioSource.PlayOneShot(clickButton);        
        helpCanvas.SetActive(true);
        helpScrollBar.value = 1;
    }

    public void OnClickSitarOffButton()
    {
        audioSource.PlayOneShot(clickSound);

        // Sitar
        isGuitar = false;
        guitarOffButton.SetActive(true);
        sitarButton.SetActive(true);
        guitarButton.SetActive(false);
        sitarOffButton.SetActive(false);
    }

    public void OnClickGuiarOffButton()
    {
        audioSource.PlayOneShot(clickSound);

        // Guitar
        isGuitar = true;
        guitarButton.SetActive(true);
        sitarOffButton.SetActive(true);        
        guitarOffButton.SetActive(false);
        sitarButton.SetActive(false);
    }

    public void OnClickResetButton()
    {
        OnClickTrashButton();
        settingCanvas.SetActive(false);
    }

    public void OnClickCloseButton()
    {
        audioSource.PlayOneShot(clickButton);
        helpCanvas.SetActive(false);
    }

    public void OnClickCrossButton()
    {
        audioSource.PlayOneShot(clickButton);        
        settingCanvas.SetActive(false);        
    }
}
