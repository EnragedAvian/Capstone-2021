using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genAController: MonoBehaviour
{
    // Global control variables
    public float globalVolume = 0.4f;
    public float volumeThresholdDistance = 10f;
    public float volumeAttenuationFactor = 1.0f;

    public List<Instrument> instruments = new List<Instrument>();
    public List<InstrumentNode> instrumentNodes = new List<InstrumentNode>();
    
    

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
