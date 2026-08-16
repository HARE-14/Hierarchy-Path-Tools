using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using static HareStudio.HierarchyPath.TextContainer;

namespace HareStudio.HierarchyPath
{
	internal static class HierarchyPathMenu
	{
		private const string MENU_ROOT = "GameObject/Hierarchy Path/";
		private const string COPY_ABSOLUTE = MENU_ROOT + "Copy Absolute Path (Avatar Root)";
		private const string COPY_RELATIVE = MENU_ROOT + "Copy Relative Path (Merge Animator)";
		private const string COPY_SCENE = MENU_ROOT + "Copy Scene Path";
		private const string OPEN_WINDOW = MENU_ROOT + "Open Window";
		private const int MENU_PRIORITY = -1000;

		private static bool queued;

		// GameObject/ 메뉴가 선택 개수만큼 호출돼서 3번 복사됐음. delayCall 로 1회만
		private static void RunOnce(Action job)
		{
			if (queued) return;
			queued = true;

			EditorApplication.delayCall += () =>
			{
				queued = false;
				job();
			};
		}

		// KeyCode.None 주면 등록이 무시됨. 인자 생략해야 미할당으로 잡힘
		[MenuItem(COPY_ABSOLUTE, false, MENU_PRIORITY)]
		[Shortcut("Hierarchy Path/Copy Absolute Path")]
		private static void CopyAbsolute() => RunOnce(() => CopySelection(PathBuilder.GetAbsolutePath, noAvatarRoot));

		[MenuItem(COPY_RELATIVE, false, MENU_PRIORITY + 1)]
		[Shortcut("Hierarchy Path/Copy Relative Path")]
		private static void CopyRelative() => RunOnce(() => CopySelection(RelativePathOf, noRelativeBasis));

		[MenuItem(COPY_SCENE, false, MENU_PRIORITY + 2)]
		[Shortcut("Hierarchy Path/Copy Scene Path")]
		private static void CopyScene() => RunOnce(() => CopySelection(PathBuilder.GetScenePath, null));

		[MenuItem(OPEN_WINDOW, false, MENU_PRIORITY + 20)]
		[Shortcut("Hierarchy Path/Open Window")]
		private static void OpenWindow() => RunOnce(HierarchyPathWindow.Open);

		[MenuItem(COPY_ABSOLUTE, true)]
		private static bool ValidateAbsolute()
		{
			var t = Selection.activeTransform;
			return t != null && PathBuilder.GetAvatarRoot(t) != null;
		}

		[MenuItem(COPY_RELATIVE, true)]
		private static bool ValidateRelative()
		{
			var t = Selection.activeTransform;
			return t != null && RelativePathOf(t) != null;
		}

		[MenuItem(COPY_SCENE, true)]
		private static bool ValidateScene() => Selection.transforms.Length > 0;

		private static string RelativePathOf(Transform t)
		{
			string basis, problem;
			return PathBuilder.GetRelativePath(t, out basis, out problem);
		}

		private static void CopySelection(Func<Transform, string> build, string emptyMessage)
		{
			var targets = Selection.transforms;
			if (targets.Length == 0) return;

			var paths = new List<string>();
			string duplicate = null;

			foreach (var t in targets)
			{
				var path = build(t);
				if (path == null) continue;

				paths.Add(path);

				if (duplicate == null)
					duplicate = PathBuilder.FindDuplicateName(t, PathBuilder.GetAvatarRoot(t));
			}

			if (paths.Count == 0)
			{
				if (emptyMessage != null) Debug.LogWarning(emptyMessage);
				return;
			}

			var joined = string.Join("\n", paths.ToArray());
			EditorGUIUtility.systemCopyBuffer = joined;

			var report = CopiedPaths(paths.Count, joined);

			if (duplicate != null)
				Debug.LogWarning(report + "\n\n" + DuplicateName(duplicate), targets[0]);
			else
				Debug.Log(report, targets[0]);
		}

	}
}
