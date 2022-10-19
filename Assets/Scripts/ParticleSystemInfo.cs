using System.Collections;
using System.Collections.Generic;
using UnityEngine;

# if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ParticleSystemInfo : MonoBehaviour
{

    public string name;

    public Color startColor = Color.green;
    public Color endColor = Color.red;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




# if UNITY_EDITOR
    public void Inspector()
    {
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            name = EditorGUILayout.TextField("Name", name);
            startColor = EditorGUILayout.ColorField("Start color", startColor);
            endColor = EditorGUILayout.ColorField("End color", endColor);
        }
    }


# endif

}
