using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR // what happens if I get rid of this?
using UnityEditor;
#endif

public class ActorParticles : MonoBehaviour
{
	public bool InspectSkeleton = false;


	public bool DrawSkeleton = true;


	public int MaxHistory = 0;
	public int Sampling = 0;

	public float BoneSize = 0.025f;
	public Color BoneColor = UltiDraw.Cyan;
	public Color JointColor = UltiDraw.Mustard;

	public Bone[] Bones = new Bone[0];

	private List<State> History = new List<State>();

	private string[] BoneNames = null;





	public float startLifetime = 1.0f;

    public GameObject ParticleSystemsPrefab;
    public List<GameObject> ParticleSystems = new List<GameObject>();

    private void Reset() //Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time. This function is only called in editor mode. Reset is most commonly used to give good default values in the Inspector.
	{
		//ParticleSystemsPrefab = Resources.Load<GameObject>("Prefabs/ParticleSystems/PS1");
		ExtractSkeleton();
   //     for (int b = 0; b < Bones.Length; b++)
   //     {
   //         ParticleSystems.Add(Instantiate(ParticleSystemsPrefab, Bones[b].Transform));
			//ParticleSystems[b].GetComponent<ParticleSystemInfo>().name = Bones[b].GetName();
   //     }
    }

	private void Start()
    {
		for (int b = 0; b < Bones.Length; b++)
		{
			ParticleSystems.Add(Instantiate(ParticleSystemsPrefab, Bones[b].Transform.position, Bones[b].Transform.rotation));

            ParticleSystem PS = ParticleSystems[b].GetComponent<ParticleSystem>();
            var sc = PS.main;

            Gradient grad = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[2];
            GradientAlphaKey[] alphaKey;

            grad.SetKeys(new GradientColorKey[7] { new GradientColorKey(Bones[b].Color1, 0.0f), new GradientColorKey(Bones[b].Color2, 0.15f) , new GradientColorKey(Bones[b].Color3, 0.35f), new GradientColorKey(Bones[b].Color4, 0.5f), new GradientColorKey(Bones[b].Color3, 0.65f), new GradientColorKey(Bones[b].Color2, 0.85f), new GradientColorKey(Bones[b].Color1, 1.0f) }, new GradientAlphaKey[2] { new GradientAlphaKey(1.0f, 0.0f),  new GradientAlphaKey(1.0f, 1.0f) });

           // sc.startLifetime = 2.0f;
            sc.startColor = grad;
			sc.startSize = Bones[b].startSize;

			sc.startLifetime = startLifetime;

			var em = PS.emission;
			em.rateOverDistance = Bones[b].rateOverDistance;


            //var col = PS.colorOverLifetime;
            //grad = new Gradient();
            //grad.SetKeys(new GradientColorKey[2] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.blue, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
            //col.color = grad;


        }
    }

    private void Update()
    {
        for (int b = 0; b < Bones.Length; b++)
        {
            ParticleSystems[b].transform.position = Bones[b].Transform.position;
            ParticleSystems[b].transform.rotation = Bones[b].Transform.rotation;
        }
    }

    private void LateUpdate()
	{
		SaveState();
	}



	public void CopySetup(ActorParticles reference)
	{
		ExtractSkeleton(reference.GetBoneNames());
	}

	public void SaveState()
	{
		if (MaxHistory > 0)
		{
			State state = new State();
			state.Transformations = GetBoneTransformations();
			state.Velocities = GetBoneVelocities();
			History.Add(state);
		}
		while (History.Count > MaxHistory)
		{
			History.RemoveAt(0);
		}
	}

	public Transform GetRoot()
	{
		// switch this to actually return transform of root bone as opposed to GameObject this is attached to?
		return transform;
	}

	public Transform[] FindTransforms(params string[] names)
	{
		Transform[] transforms = new Transform[names.Length];
		for (int i = 0; i < transforms.Length; i++)
		{
			transforms[i] = FindTransform(names[i]);
		}
		return transforms;
	}

	public Transform FindTransform(string name)
	{
		Transform element = null;
		Action<Transform> recursion = null;
		recursion = new Action<Transform>((transform) => {
			if (transform.name == name)
			{
				element = transform;
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				recursion(transform.GetChild(i));
			}
		});
		recursion(GetRoot());
		return element;
	}

	public Bone[] FindBones(params Transform[] transforms)
	{
		Bone[] bones = new Bone[transforms.Length];
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i] = FindBone(transforms[i]);
		}
		return bones;
	}

	public Bone[] FindBones(params string[] names) // By using the params keyword, you can specify a method parameter that takes a variable number of arguments. The parameter type must be a single-dimensional array.
	{
		Bone[] bones = new Bone[names.Length];
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i] = FindBone(names[i]);
		}
		return bones;
	}

	public Bone FindBone(Transform transform)
	{
		return FindBone(transform.name);
	}

	public Bone FindBone(string name)
	{
		return Array.Find(Bones, x => x.GetName() == name);
	}

	public Bone FindBoneContains(string name)
	{
		return Array.Find(Bones, x => x.GetName().Contains(name));
	}

	public string[] GetBoneNames()
	{
		if (BoneNames == null || BoneNames.Length != Bones.Length)
		{
			BoneNames = new string[Bones.Length];
			for (int i = 0; i < BoneNames.Length; i++)
			{
				BoneNames[i] = Bones[i].GetName();
			}
		}
		return BoneNames;
	}

	public int[] GetBoneIndices(params string[] names)
	{
		int[] indices = new int[names.Length];
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] = FindBone(names[i]).Index;
		}
		return indices;
	}

	public Transform[] GetBoneTransforms(params string[] names)
	{
		Transform[] transforms = new Transform[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			transforms[i] = FindTransform(names[i]);
		}
		return transforms;
	}

	public void ExtractSkeleton()
	{
		// https://docs.microsoft.com/en-us/dotnet/api/system.action-2?view=net-6.0
		// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
		ArrayExtensions.Clear(ref Bones);
		Action<Transform, Bone> recursion = null;
		recursion = (transform, parent) =>
		{
			Bone bone = new Bone(this, transform, Bones.Length);
			ArrayExtensions.Append(ref Bones, bone);
			if (parent != null)
			{
				bone.Parent = parent.Index;
				ArrayExtensions.Append(ref parent.Childs, bone.Index);
			}
			parent = bone;
			for (int i = 0; i < transform.childCount; i++)
			{
				recursion(transform.GetChild(i), parent);
			}
		};
		recursion(GetRoot(), null);
		BoneNames = new string[0];
	}

	public void ExtractSkeleton(Transform[] bones)
	{
		ArrayExtensions.Clear(ref Bones);
		Action<Transform, Bone> recursion = null;
		recursion = (transform, parent) =>
		{
			if (System.Array.Find(bones, x => x == transform))
			{
				Bone bone = new Bone(this, transform, Bones.Length);
				ArrayExtensions.Append(ref Bones, bone);
				if (parent != null)
				{
					bone.Parent = parent.Index;
					ArrayExtensions.Append(ref parent.Childs, bone.Index);
				}
				parent = bone;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				recursion(transform.GetChild(i), parent);
			}
		};
		recursion(GetRoot(), null);
		BoneNames = new string[0];
	}

	public void ExtractSkeleton(string[] bones)
	{
		ExtractSkeleton(FindTransforms(bones));
	}

	public void WriteTransforms(Matrix4x4[] values, string[] names)
	{

	}

	public void SetBoneTransformations(Matrix4x4[] values)
	{

	}

	public void SetBoneTransformations(Matrix4x4[] values, params string[] bones)
	{

	}

	public void SetBoneTransformation(Matrix4x4 value, string bone)
	{

	}

	public Matrix4x4[] GetBoneTransformations()
	{
		Matrix4x4[] transformations = new Matrix4x4[Bones.Length];
		for (int i = 0; i < Bones.Length; i++)
		{
			transformations[i] = Bones[i].Transform.GetWorldMatrix();
		}
		return transformations;
	}

	public Matrix4x4[] GetBoneTransformations(params string[] bones)
	{
		Matrix4x4[] transformations = new Matrix4x4[bones.Length];
		for (int i = 0; i < bones.Length; i++)
		{
			transformations[i] = GetBoneTransformation(bones[i]);

		}
		return transformations;
	}

	public Matrix4x4 GetBoneTransformation(string bone)
	{
		return FindBone(bone).Transform.GetWorldMatrix();
	}

	public void SetBoneVelocities(Vector3[] values)
	{

	}

	public void SetBoneVelocities(Vector3[] values, params string[] bones)
	{

	}

	public void SetBoneVelocity(Vector3 value, string bone)
	{

	}

	public Vector3[] GetBoneVelocities()
	{
		Vector3[] velocities = new Vector3[Bones.Length];
		for (int i = 0; i < Bones.Length; i++)
		{
			velocities[i] = Bones[i].Velocity;
		}
		return velocities;
	}

	public Vector3[] GetBoneVelocities(params string[] bones)
	{
		Vector3[] velocities = new Vector3[bones.Length];
		for (int i = 0; i < bones.Length; i++)
		{
			velocities[i] = GetBoneVelocity(bones[i]);
		}
		return velocities;
	}

	public Vector3 GetBoneVelocity(string bone)
	{
		return FindBone(bone).Velocity;
	}

	public Vector3[] GetBonePositions()
	{
		return null;
	}

	public Vector3[] GetBonePositions(params string[] bones)
	{
		return null;
	}

	public Vector3 GetBonePosition(string bone)
	{
		return Vector3.zero;
	}

	public Quaternion[] GetBoneRotations()
	{
		return null;
	}

	public Quaternion[] GetBoneRotations(params string[] bones)
	{
		return null;
	}

	public Quaternion GetBoneRotation(string bone)
	{
		return Quaternion.identity;
	}

	public Bone[] GetRootBones()
	{
		List<Bone> bones = new List<Bone>();
		for (int i = 0; i < Bones.Length; i++)
		{
			if (Bones[i].GetParent() == null)
			{
				bones.Add(Bones[i]);
			}
		}
		return bones.ToArray();
	}

	public void Draw()
	{
		UltiDraw.Begin();

		if (DrawSkeleton)
		{
			Action<Bone> recursion = null;
			recursion = (bone) =>
			{
				if (bone.GetParent() != null)
				{
					UltiDraw.DrawBone(bone.GetParent().Transform.position,
						//Quaternion.FromToRotation(Vector3.forward, bone.Transform.position - bone.GetParent().Transform.position),
						Quaternion.FromToRotation(bone.GetParent().Transform.forward, bone.Transform.position - bone.GetParent().Transform.position) * bone.GetParent().Transform.rotation,
						12.5f * BoneSize * bone.GetLength(),
						bone.GetLength(),
						BoneColor);
				}
				UltiDraw.DrawSphere(
					bone.Transform.position,
					Quaternion.identity,
					5.0f / 8.0f * BoneSize,
					JointColor
					);
				for (int i = 0; i < bone.Childs.Length; i++)
				{
					recursion(bone.GetChild(i));
				}
			};
			foreach (Bone bone in GetRootBones())
			{
				recursion(bone);
			};
		}
		UltiDraw.End();
	}

	private void OnRenderObject()
	{
		Draw();
	}


	public class State
	{
		public Matrix4x4[] Transformations;
		public Vector3[] Velocities;
	}

	[Serializable]
	public class Bone
	{
		public Color Color;
		public ActorParticles Actor;
		public Transform Transform;
		public Vector3 Velocity;
		public int Index;
		public int Parent;
		public int[] Childs;

		public Color Color1 = new Color(0.0f/255.0f, 24.0f / 255.0f, 255.0f / 255.0f);
		public Color Color2 = new Color(8.0f / 255.0f, 228.0f / 255.0f, 243.0f / 255.0f);
		public Color Color3 = new Color(255.0f / 255.0f, 158 / 255.0f, 6.0f / 255.0f);
		public Color Color4 = new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);	
		public float startSize = 0.1f;
		public int rateOverDistance = 60;

		public Bone(ActorParticles actor, Transform transform, int index)
		{
			Color = UltiDraw.None;
			Actor = actor;
			Transform = transform;
			Velocity = Vector3.zero;
			Index = index;
			Parent = -1;
			Childs = new int[0];
		}

		public string GetName()
		{
			return Transform.name;
		}

		public Bone GetParent()
		{
			return Parent == -1 ? null : Actor.Bones[Parent];
		}

		public Bone GetChild(int index)
		{
			return index >= Childs.Length ? null : Actor.Bones[Childs[index]];
		}

		public float GetLength()
		{
			return GetParent() == null ? 0f : Vector3.Distance(GetParent().Transform.position, Transform.position);
		}
	}

#if UNITY_EDITOR // what does this do?
	// file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.29f1/Editor/Data/Documentation/en/ScriptReference/Editor.html
	// https://docs.unity3d.com/2020.3/Documentation/Manual/editor-CustomEditors.html
	[CustomEditor(typeof(ActorParticles))]
	public class ActorParticlesEditor : Editor
	{
		public ActorParticles Target;

		private void Awake()
		{
			Target = (ActorParticles)target;
		}

		public override void OnInspectorGUI()
		{
			Undo.RecordObject(Target, Target.name); // not sure what this does file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.29f1/Editor/Data/Documentation/en/ScriptReference/Undo.RecordObject.html

			EditorGUILayout.HelpBox("This is base particle system you wish to use", MessageType.None);
			Target.ParticleSystemsPrefab = (GameObject)EditorGUILayout.ObjectField("Base Particle System", Target.ParticleSystemsPrefab, typeof(GameObject), true);
			EditorGUILayout.HelpBox("This is essentially how long the trail will last", MessageType.None);
			Target.startLifetime = EditorGUILayout.Slider("Start Life TIme", Target.startLifetime, 0.1f, 10.0f);

			Target.DrawSkeleton = EditorGUILayout.Toggle("Draw Skeleton", Target.DrawSkeleton);


			Utility.SetGUIColor(Color.grey);
			using (new EditorGUILayout.VerticalScope("box")) // https://docs.unity3d.com/2020.3/Documentation/ScriptReference/GUILayout.VerticalScope.html
			{
				Utility.ResetGUIColor();
				if (Utility.GUIButton("Joint Selection", UltiDraw.DarkGrey, UltiDraw.White))
				{
					Target.InspectSkeleton = !Target.InspectSkeleton;
				}
				if (Target.InspectSkeleton)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Particle systems: " + Target.Bones.Length);
					if (Utility.GUIButton("Clear", UltiDraw.DarkGrey, UltiDraw.White))
					{
						Target.ExtractSkeleton(new Transform[0]);
					}
					EditorGUILayout.EndHorizontal();
					InspectSkeleton(Target.GetRoot(), 0);
				}
			}

			if (GUI.changed) // not sure of this loop's purpose
			{
				EditorUtility.SetDirty(Target);
			}
		}

		private void InspectSkeleton(Transform transform, int indent) // not 100% sure on exactly how everything here is working
		{
			float indentSpace = 5.0f;
			Bone bone = Target.FindBone(transform.name);
			Utility.SetGUIColor(bone == null ? UltiDraw.LightGrey : UltiDraw.Mustard);

			using (new EditorGUILayout.VerticalScope("Box"))
			{
				Utility.ResetGUIColor();
				EditorGUILayout.BeginHorizontal();
				for (int i = 0; i < indent; i++)
				{
					EditorGUILayout.LabelField("|", GUILayout.Width(indentSpace));
				}
				EditorGUILayout.LabelField("-", GUILayout.Width(indentSpace));
				EditorGUILayout.LabelField(transform.name, GUILayout.Width(400f - indent * indentSpace));
				GUILayout.FlexibleSpace();
				if (bone != null)
				{
					EditorGUILayout.LabelField("Index: " + bone.Index.ToString(), GUILayout.Width(60f));
				}

				if (Utility.GUIButton("Bone", bone == null ? UltiDraw.White : UltiDraw.DarkGrey, bone == null ? UltiDraw.DarkGrey : UltiDraw.White))
				{
					Transform[] bones = new Transform[Target.Bones.Length];
					for (int i = 0; i < bones.Length; i++)
					{
						bones[i] = Target.Bones[i].Transform;
					}
					if (bone == null)
					{
						ArrayExtensions.Append(ref bones, transform);
						Target.ExtractSkeleton(bones);

					}
					else
					{
						ArrayExtensions.Remove(ref bones, transform);
						Target.ExtractSkeleton(bones);
					}
				}
				EditorGUILayout.EndHorizontal();
				if (bone != null)
				{
					Target.Bones[bone.Index].Color1 = EditorGUILayout.ColorField("Color1", Target.Bones[bone.Index].Color1);
					Target.Bones[bone.Index].Color2 = EditorGUILayout.ColorField("Color2", Target.Bones[bone.Index].Color2);
					Target.Bones[bone.Index].Color3 = EditorGUILayout.ColorField("Color3", Target.Bones[bone.Index].Color3);
					Target.Bones[bone.Index].Color4 = EditorGUILayout.ColorField("Color4", Target.Bones[bone.Index].Color4);
					Target.Bones[bone.Index].startSize = EditorGUILayout.Slider("Start Size", Target.Bones[bone.Index].startSize, 0.0f, 1.0f); //FloatField("Start Size", Target.Bones[bone.Index].startSize);
					Target.Bones[bone.Index].rateOverDistance = EditorGUILayout.IntSlider("Rate over distance", Target.Bones[bone.Index].rateOverDistance, 0, 100);
				}



			}

			for (int i = 0; i < transform.childCount; i++)
			{
				InspectSkeleton(transform.GetChild(i), indent + 1);
			}
		}
	}
#endif

}
