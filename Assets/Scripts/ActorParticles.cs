using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR // what happens if I get rid of this?
using UnityEditor;
#endif

public class ActorParticles : MonoBehaviour
{
	public bool InspectSkeleton = false;

	public bool DrawRoot = false;
	public bool DrawSkeleton = true;
	public bool DrawSketch = false;
	public bool DrawTransforms = false;
	public bool DrawVelocities = false;
	public bool DrawHistory = false;

	public int MaxHistory = 0;
	public int Sampling = 0;

	public float BoneSize = 0.025f;
	public Color BoneColor = UltiDraw.Cyan;
	public Color JointColor = UltiDraw.Mustard;

	public Bone[] Bones = new Bone[0];

	private List<State> History = new List<State>();

	private string[] BoneNames = null;

	private List<List<GameObject>> PastAvatars = new List<List<GameObject>>();
	private List<GameObject> CurrentAvater = new List<GameObject>();

	GameObject sphere;


	
	public float PastWindow = 1.0f;
	public int NumPastPoints = 10;
	private float Interval;
	private System.DateTime TimeLastUpdate;
	private bool changed = false;

	public float minSize = 0.01f;
	public float maxSize = 0.05f;

	public Color StartColour = Color.blue;
	public Color EndColour = Color.black;


	private void Reset() //Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time. This function is only called in editor mode. Reset is most commonly used to give good default values in the Inspector.
	{
		ExtractSkeleton();
	}

    private void Start()
    {
		

		Interval = PastWindow / (int)NumPastPoints;
		Debug.Log(Interval);
		TimeLastUpdate = Utility.GetTimestamp();


		for (int b = 0; b < Bones.Length; b++)
        {
		
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.name = "Sphere_0" + "_" + b.ToString();
			sphere.transform.position = Bones[b].Transform.position;
            sphere.transform.rotation = Bones[b].Transform.rotation;
            sphere.transform.localScale = new Vector3(maxSize, maxSize, maxSize);
			CurrentAvater.Add(sphere);

			var cubeRenderer = sphere.GetComponent<Renderer>();
			
			cubeRenderer.material.SetColor("_Color", StartColour);


		}

        //Debug.Log(CurrentAvater.Count);
        //for (int b = 0; b < Bones.Length; b++)
        //{
        //    TrailRenderer trailRenderer = CurrentAvater[b].AddComponent<TrailRenderer>();
        //    trailRenderer.time = 1.0f;
        //}




        for (int i = 0; i < NumPastPoints; i++)
        {
			float scale = ((float)i / (float)NumPastPoints) * maxSize + (((float)NumPastPoints - (float)i) / (float)NumPastPoints) * minSize;
			Color customColor = Color.Lerp(EndColour, StartColour, (float)i / (float)NumPastPoints);
			Debug.Log(scale);

			List<GameObject> Spheres = new List<GameObject>();
            for (int b = 0; b < Bones.Length; b++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "Sphere_" + i.ToString() + "_" + b.ToString();
                sphere.transform.position = Bones[b].Transform.position;
                sphere.transform.rotation = Bones[b].Transform.rotation;

				
				
                //sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
				sphere.transform.localScale = new Vector3(scale, scale, scale);

				var cubeRenderer = sphere.GetComponent<Renderer>();
				
				cubeRenderer.material.SetColor("_Color", customColor);

				Spheres.Add(sphere);
            }
            PastAvatars.Add(Spheres);
        }
        //{
        //    Debug.Log("hi");
        //    List<GameObject> Spheres = new List<GameObject>();
        //    for (int b = 0; b < Bones.Length; b++)
        //    {
        //        Debug.Log("bye");
        //        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        sphere.transform.position = Bones[b].Transform.position;
        //        sphere.transform.rotation = Bones[b].Transform.rotation;
        //        sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);


        //        MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();

        //        //if (i == 0)
        //        //            {
        //        //	Material newMat = Resources.Load("Assets/Scripts/Materials/ShinyTest", typeof(Material)) as Material;
        //        //	sphere.GetComponent<Renderer>().sharedMaterial = newMat;
        //        //	//sphere.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
        //        //	//sphere.renderer.shared = newMat;
        //        //}
        //        //            if (i > 0)
        //        //            {
        //        //	Material newMat1 = Resources.Load("Assets/Scripts/Materials/ShinyTest1", typeof(Material)) as Material;
        //        //	sphere.GetComponent<Renderer>().sharedMaterial = newMat1;
        //        //}


        //        Spheres.Add(sphere);
        //    }
        //    PastAvatars.Add(Spheres);
        //}
        //sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    }


	private void OnDIsable()
    {
		//for (int i = 0; i < MaxHistory; i++)
		//{
		//    List<GameObject> Spheres = PastAvatars[0];
		//    PastAvatars.RemoveAt(0);
		//    for (int b = 0; b < Bones.Length; b++)
		//    {
		//        GameObject sphere = Spheres[0];
		//        Spheres.RemoveAt(0);
		//        Destroy(sphere.transform.gameObject);
		//    }
		//}
		//Destroy(sphere.transform.gameObject);
	}

    private void LateUpdate()
	{
        // does this work for updating as required . . .  .
        //float ElapsedTime = (float)Utility.GetElapsedTime(TimeLastUpdate);

        //if (ElapsedTime >= Interval)
        //{

        //    TimeLastUpdate = Utility.GetTimestamp();
        //    SaveState();
        //    //changed = true;
        //}
        //else
        //{

        //    changed = false;
        //}

		SaveState();

        //for (int i = 0; i < MaxHistory; i++)
        //{
        //    State state = History[i];
        //    for (int b = 0; b < Bones.Length; b++)
        //    {
        //        PastAvatars[i][b].transform.position = state.Transformations[b].GetPosition();
        //        PastAvatars[i][b].transform.rotation = state.Transformations[b].GetRotation();
        //        // do something with velocities???
        //    }
        //}

  //      for (int b = 0; b < Bones.Length; b++)
		//{
		//	CurrentAvater[b].transform.position = Bones[b].Transform.position; 
		//	CurrentAvater[b].transform.rotation = Bones[b].Transform.rotation; 
		//	// do something with velocities???
		//}


	}

    private void Update()
    {
		Utility.SetFPS(90);
		for (int b = 0; b < Bones.Length; b++)
		{
			CurrentAvater[b].transform.position = Bones[b].Transform.position;
			CurrentAvater[b].transform.rotation = Bones[b].Transform.rotation;
			// do something with velocities???
		}


		//if (changed)
  //      {
            for (int i = 0; i < History.Count; i++)
            {
                State state = History[i];
                for (int b = 0; b < Bones.Length; b++)
                {
                    PastAvatars[i][b].transform.position = state.Transformations[b].GetPosition();
                    PastAvatars[i][b].transform.rotation = state.Transformations[b].GetRotation();
                    // do something with velocities???
                }
            }
       // }






    }

	public void CopySetup(ActorParticles reference)
	{
		ExtractSkeleton(reference.GetBoneNames());
	}

	public void SaveState()
	{
		if (NumPastPoints > 0.0)
		{
			State state = new State();
			state.Transformations = GetBoneTransformations();
			state.Velocities = GetBoneVelocities();
			History.Add(state);
		}
		if (History.Count > NumPastPoints)
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
		if (DrawRoot)
		{
			UltiDraw.DrawCube(GetRoot().position, GetRoot().rotation, 0.1f, UltiDraw.Black);
			UltiDraw.DrawTranslateGizmo(GetRoot().position, GetRoot().rotation, 0.1f);
		}

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

		if (DrawVelocities)
		{
			for (int i = 0; i < Bones.Length; i++)
			{
				UltiDraw.DrawArrow(
					Bones[i].Transform.position,
					Bones[i].Transform.position + Bones[i].Velocity,
					0.75f,
					0.0075f,
					0.05f,
					UltiDraw.DarkGreen.Opacity(0.5f)
				);
			}
		}

		if (DrawTransforms)
		{
			Action<Bone> recursion = null;
			recursion = (bone) => {
				UltiDraw.DrawTranslateGizmo(
					bone.Transform.position,
					bone.Transform.rotation,
					0.05f);
				for (int i = 0; i < bone.Childs.Length; i++)
				{
					recursion(bone.GetChild(i));
				}
			};
			foreach (Bone bone in GetRootBones())
			{
				recursion(bone);
			}
		}
		UltiDraw.End();

		if (DrawSketch)
		{
			Sketch(GetBoneTransformations(), BoneColor);
		}

		if (DrawHistory)
		{
			if (DrawSkeleton)
			{
				int step = Mathf.Max(Sampling, 1);
				for (int i = 0; i < History.Count; i += step)
				{
					Sketch(History[i].Transformations, BoneColor.Darken(1f - (float)i / (float)History.Count));
				}
			}
			if (DrawVelocities)
			{
				float max = 0f;
				float[][] functions = new float[History.Count][];
				for (int i = 0; i < History.Count; i++)
				{
					functions[i] = new float[Bones.Length];
					for (int j = 0; j < Bones.Length; j++)
					{
						functions[i][j] = History[i].Velocities[j].magnitude;
						max = Mathf.Max(max, functions[i][j]);
					}
				}
				UltiDraw.Begin();
				UltiDraw.PlotFunctions(new Vector2(0.5f, 0.05f), new Vector2(0.9f, 0.1f), functions, UltiDraw.Dimension.Y, yMin: 0f, yMax: max, thickness: 0.0025f);
				UltiDraw.End();
			}
		}
	}

	public void Draw(Matrix4x4[] transformations, Color color)
	{
		UltiDraw.Begin();
		if (transformations.Length != Bones.Length)
		{
			Debug.Log("Number of given transformations does not match number of bones.");
		}
		else
		{
			Action<Bone> recursion = null;
			recursion = (bone) => {
				Matrix4x4 current = transformations[bone.Index];
				if (bone.GetParent() != null)
				{
					Matrix4x4 parent = transformations[bone.GetParent().Index];
					UltiDraw.DrawBone(
						parent.GetPosition(),
						Quaternion.FromToRotation(parent.GetForward(), current.GetPosition() - parent.GetPosition()) * parent.GetRotation(),
						12.5f * BoneSize * bone.GetLength(), bone.GetLength(),
						color
					);
				}
				UltiDraw.DrawSphere(
					current.GetPosition(),
					Quaternion.identity,
					5f / 8f * BoneSize,
					color
				);
				for (int i = 0; i < bone.Childs.Length; i++)
				{
					recursion(bone.GetChild(i));
				}
			};
			foreach (Bone bone in GetRootBones())
			{
				recursion(bone);
			}
		}
		UltiDraw.End();
	}

	public void Sketch(Matrix4x4[] transformations, Color color)
	{
		UltiDraw.Begin();
		if (transformations.Length != Bones.Length)
		{
			Debug.Log("Number of given transformations does not match number of bones.");
		}
		else
		{
			Action<Bone> recursion = null;
			recursion = (bone) => {
				Matrix4x4 current = transformations[bone.Index];
				if (bone.GetParent() != null)
				{
					Matrix4x4 parent = transformations[bone.GetParent().Index];
					UltiDraw.DrawLine(parent.GetPosition(), current.GetPosition(), color);
				}
				UltiDraw.DrawCube(current.GetPosition(), current.GetRotation(), 0.02f, color);
				for (int i = 0; i < bone.Childs.Length; i++)
				{
					recursion(bone.GetChild(i));
				}
			};
			foreach (Bone bone in GetRootBones())
			{
				recursion(bone);
			}
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

			Target.PastWindow = EditorGUILayout.Slider("Window Length (seconds)", Target.PastWindow, 0.0f, 10.0f);
			Target.NumPastPoints = EditorGUILayout.IntSlider("Number of steps", Target.NumPastPoints, 1, 50);
			EditorGUILayout.LabelField("Time interval (seconds)", Target.Interval.ToString());

			Target.minSize = EditorGUILayout.Slider("min sphere size", Target.minSize, 0.0f, 1.0f);
			Target.maxSize = EditorGUILayout.Slider("max sphere size", Target.maxSize, 0.0f, 1.0f);

			Target.DrawRoot = EditorGUILayout.Toggle("Draw Root", Target.DrawRoot);
			Target.DrawSkeleton = EditorGUILayout.Toggle("Draw Skeleton", Target.DrawSkeleton);
			Target.DrawSketch = EditorGUILayout.Toggle("Draw Sketch", Target.DrawSketch);
			Target.DrawTransforms = EditorGUILayout.Toggle("Draw Transforms", Target.DrawTransforms);
			Target.DrawVelocities = EditorGUILayout.Toggle("Draw Velocities", Target.DrawVelocities);
			Target.DrawHistory = EditorGUILayout.Toggle("Draw History", Target.DrawHistory);

			Target.MaxHistory = EditorGUILayout.IntField("Max History", Target.MaxHistory);
			Target.Sampling = EditorGUILayout.IntField("Sampling", Target.Sampling);

			Utility.SetGUIColor(Color.grey);
			using (new EditorGUILayout.VerticalScope("box")) // https://docs.unity3d.com/2020.3/Documentation/ScriptReference/GUILayout.VerticalScope.html
			{
				Utility.ResetGUIColor();
				if (Utility.GUIButton("Skeleton", UltiDraw.DarkGrey, UltiDraw.White))
				{
					Target.InspectSkeleton = !Target.InspectSkeleton;
				}
				if (Target.InspectSkeleton)
				{
					ActorParticles reference = (ActorParticles)EditorGUILayout.ObjectField("Reference", null, typeof(ActorParticles), true);
					if (reference != null)
					{
						Target.CopySetup(reference);
					}
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Bones: " + Target.Bones.Length);
					if (Utility.GUIButton("Clear", UltiDraw.DarkGrey, UltiDraw.White))
					{
						Target.ExtractSkeleton(new Transform[0]);
					}
					EditorGUILayout.EndHorizontal();
					Target.BoneSize = EditorGUILayout.FloatField("Bone size", Target.BoneSize);
					Target.StartColour = EditorGUILayout.ColorField("StartColour", Target.StartColour);
					Target.EndColour = EditorGUILayout.ColorField("End color", Target.EndColour);
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

			using (new EditorGUILayout.HorizontalScope("Box"))
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
					EditorGUILayout.LabelField("Length: " + bone.GetLength().ToString("F3"), GUILayout.Width(90f));
					bone.Color = EditorGUILayout.ColorField(bone.Color);
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
			}

			for (int i = 0; i < transform.childCount; i++)
			{
				InspectSkeleton(transform.GetChild(i), indent + 1);
			}
		}
	}
#endif

}
