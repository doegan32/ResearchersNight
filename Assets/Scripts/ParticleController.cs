using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
# endif


// switch to actor script in end 
[RequireComponent(typeof(ActorParticles))]
public class ParticleController : MonoBehaviour
{

    public GameObject ParticleSystemsPrefab;
    private List<GameObject> ParticleSystems = new List<GameObject>();


    private ActorParticles Actor;

    // Start is called before the first frame update
    void Start()
    {
        Actor = GetComponent<ActorParticles>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int b = 0; b < Actor.Bones.Length; b++)
        {
            Actor.ParticleSystems[b].transform.position = Actor.Bones[b].Transform.position;
            Actor.ParticleSystems[b].transform.rotation = Actor.Bones[b].Transform.rotation;
        }
    }








//#if UNITY_EDITOR
//    [CustomEditor(typeof(ParticleController))]
//    public class ParticleController_Editor : Editor
//    {
//        public ParticleController Target;

//        private void Awake()
//        {
//            Target = (ParticleController)target;
//        }


//        public override void OnInspectorGUI()
//        {
//            Undo.RecordObject(Target, Target.name);

//            Target.ParticleSystemsPrefab = (GameObject)EditorGUILayout.ObjectField("Base Particle System", Target.ParticleSystemsPrefab, typeof(GameObject), true);

//            for (int i = 0; i < Target.ParticleSystems.Count; i++)
//            {
//                Target.ParticleSystems[i].GetComponent<ParticleSystemInfo>().Inspector();
//            }

//            if (GUI.changed)
//            {
//                EditorUtility.SetDirty(Target);
//            }
//        }

//    }


//#endif










}
