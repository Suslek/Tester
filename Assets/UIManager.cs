using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    public Canvas main;   
    public Canvas dotByDotCanvas;    
    public Canvas NoiseCanvas;
    public Canvas LVDSCanvas;

    public TMP_Dropdown mode;
    public TMP_Dropdown resolutionsDropdown;
    public TMP_InputField TargetFPS;

    public TesterScript TesterScript;

    public TMP_Dropdown[] dropdowns;
    public TMP_InputField[] inputFields;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private int currentResolutionIndex = 0;
    private float currentRefreshRate;

    [SerializeField]
    private int BPC = 8;

    private string R = "";
    private string G = "";
    private string B = "";

    private bool isMonitorOn = true;
    private bool twobit = true;

    private Dictionary<string, int> namesToIndex = new Dictionary<string,int>();

    private List<int> colorsList = new List<int>();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    // Start is called before the first frame update
    void Start()
    {
        fillNamesToIndex();
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionsDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();

        for(int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + "Hz";
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();


        UpdateWhite("1");
        UpdateBlack("1");
    }

    private void fillNamesToIndex()
    {
        for (int i = 0; i < BPC; i++)
        {
            namesToIndex.Add("R" + i.ToString(), i); 
        }
        for (int i = BPC; i < BPC * 2; i++)
        {
            namesToIndex.Add("G" + (i - BPC).ToString(), i);
        }
        for (int i = BPC * 2; i < BPC * 3; i++)
        {
            namesToIndex.Add("B" + (i - BPC * 2).ToString(), i);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnESCPress();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Monitor();
        }
    }

    public void OnApplyPress()
    {
        main.gameObject.SetActive(false);
    }

    public void OnESCPress()
    {
        if (main.gameObject.activeInHierarchy)
        {
            main.gameObject.SetActive(false);
        }
        else
        {
            main.gameObject.SetActive(true);
        }
    }

    public void ModeSelector(int value)
    {

        if (value == 0)
        {
            dotByDotCanvas.gameObject.SetActive(true);
            NoiseCanvas.gameObject.SetActive(false);
            LVDSCanvas.gameObject.SetActive(false);
        }
        if (value == 1)
        {
            dotByDotCanvas.gameObject.SetActive(false);
            NoiseCanvas.gameObject.SetActive(true);
            LVDSCanvas.gameObject.SetActive(false);
        }
        if (value == 2)
        {
            dotByDotCanvas.gameObject.SetActive(false);
            NoiseCanvas.gameObject.SetActive(false);
            LVDSCanvas.gameObject.SetActive(true);
        }

        TesterScript.SetMode(value);
        
    }

    public void ChangeFPS(string value)
    {
        Application.targetFrameRate = (System.Convert.ToInt32(value));
    }

    private void UpdateColor(string R, string G, string B)
    {
        float del = (Mathf.Pow(2f, BPC) - 1);
        float Rval1 = Convert.ToInt32(R, 2);
        float Gval1 = Convert.ToInt32(G, 2);
        float Bval1 = Convert.ToInt32(B, 2);

        float Rval2 = del - Rval1;
        float Gval2 = del - Gval1;
        float Bval2 = del - Bval1;

        Debug.Log(Rval1 + " " + Gval1 + " " + Bval1);
        Debug.Log(Rval2 + " " + Gval2 + " " + Bval2);

        TesterScript.SetColors(Rval1 / del, Gval1 / del, Bval1 / del);
        if (!twobit)
        {
            TesterScript.SetColors1(Rval1 / del, Gval1 / del, Bval1 / del);
        }
        else
        {
            TesterScript.SetColors1(Rval2 / del, Gval2 / del, Bval2 / del);
        }
        
    }

    public void UpdateWhite(string value)
    {
        TesterScript.SetWhite(System.Convert.ToInt32(value));
    }

    public void UpdateBlack(string value)
    {
        TesterScript.SetBlack(System.Convert.ToInt32(value));
    }

    public void Monitor()
    {
        if (isMonitorOn)
        {
            IntPtr HWND_BROADCAST = (IntPtr)0xffff;
            uint WM_SYSCOMMAND = 0x0112;
            IntPtr SC_MONITORPOWER = (IntPtr)0xF170;
            SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, (IntPtr)2);
            isMonitorOn = false;
        }
        else
        {
            IntPtr HWND_BROADCAST = (IntPtr)0xffff;
            uint WM_SYSCOMMAND = 0x0112;
            IntPtr SC_MONITORPOWER = (IntPtr)0xF170;
            SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, (IntPtr)(-1));
            isMonitorOn = true;
        }
    }

    public void Vsync(bool value)
    {
        if (value)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void UpdateValues()
    {
        colorsList.Clear();
        for (int i = 0; i < BPC * 3; i++)
        {
            colorsList.Add(0);
        }
        
        for (int i = 0; i < dropdowns.Length; i++)
        {
            try
            {
                colorsList[Convert.ToInt32((namesToIndex[(dropdowns[i].options[dropdowns[i].value].text)]))] = Convert.ToInt32(inputFields[i].text,2);           
            }
            catch
            {

            }
        }
    }

    private void fillColorsString()
    {
        R = new string("");
        G = new string("");
        B = new string("");

        for (int i = BPC - 1; i >= 0; i--)
        {
            R += colorsList[i].ToString();
        }
        for (int i = BPC * 2 - 1; i >= BPC; i--)
        {
            G += colorsList[i].ToString();
        }
        for (int i = BPC * 3 - 1; i >= BPC * 2; i--)
        {
            B += colorsList[i].ToString();
        }
    }

    public void Apply()
    {
        UpdateValues();
        fillColorsString();
        UpdateColor(R, G, B);
    }

    public void TwoBit(bool value)
    {
        twobit = value;
    }
}
