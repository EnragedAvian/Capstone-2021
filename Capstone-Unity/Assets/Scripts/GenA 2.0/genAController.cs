using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class genAController : MonoBehaviour
{
    // Global control variables
    public float globalVolume = 0.4f;
    public float volumeThresholdDistance = 10f;
    public float volumeAttenuationFactor = 1.0f;

    public List<Instrument> instruments = new List<Instrument>();
    private Dictionary<Instrument, List<InstrumentNode>> instrumentNodes = new Dictionary<Instrument, List<InstrumentNode>>();
    private Dictionary<Instrument, Vector3> instrumentDirections = new Dictionary<Instrument, Vector3>();
    private Dictionary<Instrument, float> instrumentVolumes = new Dictionary<Instrument, float>();

    public GameObject hearingSource;

    public bool debug = false;
    public float debugVectorSize = 1f;
    public float debugVectorOffset = 1f;

    private Dictionary<Instrument, List<Vector3>> debugVectors = new Dictionary<Instrument, List<Vector3>>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Instrument myInstrument in instruments)
        {
            instrumentDirections.Add(myInstrument, new Vector3(0, 0, 0));
            instrumentVolumes.Add(myInstrument, 0f);
            instrumentNodes.Add(myInstrument, new List<InstrumentNode>());

            debugVectors.Add(myInstrument, new List<Vector3>());
        }

        foreach (InstrumentNode myObj in GameObject.FindObjectsOfType(typeof(InstrumentNode)))
        {
            print(myObj);
            List<InstrumentNode> tempArray = new List<InstrumentNode>();

            if (instrumentNodes.TryGetValue(myObj.instrument, out tempArray))
            {
                tempArray.Add(myObj);
                print("Node added!");
            }
            
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVolumes();

        if (debug)
        {
            foreach (Instrument myInstrument in instruments)
            {
                Vector3 resultantVector = instrumentDirections[myInstrument].normalized * instrumentVolumes[myInstrument];
                Debug.DrawRay(hearingSource.transform.position, resultantVector.normalized * debugVectorOffset + resultantVector * debugVectorSize, Color.blue, 0, true);
            }
        }
    }

    private void CalculateVolumes()
    {
        foreach (Instrument myInstrument in instruments)
        {
            float resultantVolume = 0;
            Vector3 resultantVolumeDirection = new Vector3(0, 0, 0);
            List<InstrumentNode> allNodes;

            if (instrumentNodes.TryGetValue(myInstrument, out allNodes)) {

                // Only used for debug purposes
                List<Vector3> allVectors = new List<Vector3>();
                if (debug && debugVectors.TryGetValue(myInstrument, out allVectors))
                {
                    allVectors.Clear();
                }
                foreach (InstrumentNode myNode in allNodes)
                {
                    Vector3 distance = myNode.transform.position - hearingSource.transform.position;

                    if (distance.magnitude < 2 * volumeThresholdDistance)
                    {
                        Vector3 newVolume = CalculateNodeVolume(distance);
                        if (debug)
                        {
                            allVectors.Add(newVolume);
                        }

                        resultantVolume += newVolume.magnitude;
                        resultantVolumeDirection += newVolume;
                        
                    }
                }
            }

            instrumentVolumes[myInstrument] = resultantVolume;
            instrumentDirections[myInstrument] = resultantVolumeDirection;
        }
    }

    private Vector3 CalculateNodeVolume(Vector3 displacement)
    {
        Vector3 nodeVolume = new Vector3(0, 0, 0);

        if (displacement.magnitude > volumeThresholdDistance)
        {
            float thresholdVolume = 1 / (float)Math.Pow((volumeAttenuationFactor * volumeThresholdDistance + 1), 2);
            nodeVolume = MapVectorMagnitude(displacement, volumeThresholdDistance, volumeThresholdDistance * 2, thresholdVolume, 0f);

            if (debug)
            {
                Debug.DrawRay(hearingSource.transform.position, displacement, Color.yellow, 0, true);
                Debug.DrawRay(hearingSource.transform.position, nodeVolume.normalized * debugVectorOffset + nodeVolume * debugVectorSize, Color.red, 0, true);
            }   
        }
        else
        {
            nodeVolume = inverseSquare(displacement);

            if (debug)
            {
                Debug.DrawRay(hearingSource.transform.position, displacement, Color.green, 0, true);
                Debug.DrawRay(hearingSource.transform.position, nodeVolume.normalized * debugVectorOffset + nodeVolume * debugVectorSize, Color.red, 0, true);
            }
        }

        return nodeVolume;
    }

    private static Vector3 MapVectorMagnitude(Vector3 value, float from1, float to1, float from2, float to2)
    {   
        float mappedMagnitude = (value.magnitude - from1) / (to1 - from1) * (to2 - from2) + from2;
        Vector3 normalizedValue = value.normalized;

        return normalizedValue * mappedMagnitude;
    }

    private Vector3 inverseSquare(Vector3 value)
    {
        float magnitude = value.magnitude;
        float volume = 1 / (float)Math.Pow(volumeAttenuationFactor * magnitude + 1, 2);

        Vector3 normal = value.normalized;


        // BAD CODE DON'T USE
        /*float _x = 1 / (float)Math.Pow(volumeAttenuationFactor * Math.Abs(value.x) + 1, 2);
        float _y = 1 / (float)Math.Pow(volumeAttenuationFactor * Math.Abs(value.y) + 1, 2);
        float _z = 1 / (float)Math.Pow(volumeAttenuationFactor * Math.Abs(value.z) + 1, 2);

        // preserving magnitude direction from origianl vector
        if (value.x < 0) { _x *= -1; }
        if (value.y < 0) { _y *= -1; }
        if (value.z < 0) { _z *= -1; }*/

        return normal * volume;
    }



}


