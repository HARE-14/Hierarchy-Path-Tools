using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using static HareStudio.HierarchyPath.TextContainer;

namespace HareStudio.HierarchyPath
{
	internal static class PathBuilder
	{
		private const string DESCRIPTOR_TYPE = "VRC.SDK3.Avatars.Components.VRCAvatarDescriptor";
		private const string MERGE_ANIMATOR_TYPE = "nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator";

		private static Type descriptorType, mergeAnimatorType;
		private static FieldInfo pathModeField, pathRootField;
		private static bool typesSearched;

		private static void SearchTypes()
		{
			if (typesSearched) return;
			typesSearched = true;

			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					if (descriptorType == null) descriptorType = asm.GetType(DESCRIPTOR_TYPE, false);
					if (mergeAnimatorType == null) mergeAnimatorType = asm.GetType(MERGE_ANIMATOR_TYPE, false);
				}
				catch { }
			}

			if (mergeAnimatorType == null) return;

			pathModeField = mergeAnimatorType.GetField("pathMode");
			pathRootField = mergeAnimatorType.GetField("relativePathRoot");

			if (pathModeField == null) mergeAnimatorType = null;
		}

		internal static Transform GetAvatarRoot(Transform t)
		{
			SearchTypes();

			if (descriptorType != null)
			{
				for (var c = t; c; c = c.parent)
					if (c.GetComponent(descriptorType)) return c;
			}

			for (var c = t; c; c = c.parent)
				if (c.GetComponent<Animator>()) return c;

			return null;
		}

		internal static string GetScenePath(Transform t)
		{
			return JoinNames(t, null);
		}

		// 루트 없으면 씬 경로랑 같은 값이 나와서 null 처리
		internal static string GetAbsolutePath(Transform t)
		{
			var root = GetAvatarRoot(t);
			return root == null ? null : JoinNames(t, root);
		}

		internal static string GetRelativePath(Transform t, out string basis, out string problem)
		{
			SearchTypes();

			basis = null;
			problem = null;

			if (mergeAnimatorType != null)
			{
				Component merge = null;
				for (var c = t; c != null && merge == null; c = c.parent)
					merge = c.GetComponent(mergeAnimatorType);

				if (merge != null) return FromMergeAnimator(merge, t, out basis, out problem);
			}

			var nested = GetNestedAnimator(t);
			if (nested == null)
			{
				problem = noRelativeBasis;
				return null;
			}

			basis = AnimatorBasis(nested.name);
			return JoinNames(t, nested);
		}

		private static string FromMergeAnimator(Component merge, Transform t, out string basis, out string problem)
		{
			basis = null;
			problem = null;

			var mode = pathModeField.GetValue(merge);
			if (mode == null || Enum.GetName(pathModeField.FieldType, mode) != "Relative")
			{
				problem = PathModeIsAbsolute(merge.name);
				return null;
			}

			var root = GetRelativePathRoot(merge, t);

			var isAbove = false;
			for (var c = t; c && !isAbove; c = c.parent) isAbove = c == root;


			basis = MergeAnimatorBasis(root.name);
			return JoinNames(t, isAbove ? root : merge.transform);
		}

		private static Transform GetRelativePathRoot(Component merge, Transform t)
		{
			if (pathRootField == null) return merge.transform;

			try
			{
				var reference = pathRootField.GetValue(merge);
				if (reference == null) return merge.transform;

				var getter = reference.GetType().GetMethod("Get", new[] { typeof(Component) });
				var avatar = GetAvatarRoot(t);
				if (getter == null || avatar == null) return merge.transform;

				var go = getter.Invoke(reference, new object[] { avatar }) as GameObject;
				return go == null ? merge.transform : go.transform;
			}
			catch
			{
				return merge.transform;
			}
		}

		private static Transform GetNestedAnimator(Transform t)
		{
			var avatar = GetAvatarRoot(t);

			for (var c = t; c; c = c.parent)
			{
				if (!c.GetComponent<Animator>()) continue;

				// 이거 없으면 상대 경로에 절대 경로랑 같은 게 뜸
				return c == avatar ? null : c;
			}

			return null;
		}

		internal static string FindDuplicateName(Transform t, Transform stopAt)
		{
			for (var c = t; c && c != stopAt; c = c.parent)
			{
				var parent = c.parent;
				var sameName = 0;

				if (parent)
				{
					for (int i = 0; i < parent.childCount; i++)
						if (parent.GetChild(i).name == c.name) sameName++;
				}
				else if (c.gameObject.scene.IsValid())
				{
					foreach (var go in c.gameObject.scene.GetRootGameObjects())
						if (go.name == c.name) sameName++;
				}

				if (sameName > 1) return c.name;
			}

			return null;
		}

		private static string JoinNames(Transform t, Transform stopAt)
		{
			if (!t || t == stopAt) return string.Empty;

			var builder = new StringBuilder(t.name);
			for (var c = t.parent; c && c != stopAt; c = c.parent)
				builder.Insert(0, c.name + "/");

			return builder.ToString();
		}

	}
}
