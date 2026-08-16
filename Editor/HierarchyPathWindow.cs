using UnityEditor;
using UnityEngine;
using static HareStudio.HierarchyPath.TextContainer;

namespace HareStudio.HierarchyPath
{
	internal class HierarchyPathWindow : EditorWindow
	{
		private const string PRODUCT_NAME = "Hierarchy Path";
		private const string USAGE_PREFS_KEY = "HierarchyPath.UsageOpen";
		private const float MIN_LABEL_WIDTH = 120f;
		private const float PATH_COLUMN_ROOM = 210f;
		private const int USAGE_FONT_SIZE = 12;//폰트는 12로

		private static Vector2 scroll;
		private static bool usageFoldout = true;
		private static GUIStyle usageStyle;

		private Transform lastTarget;
		private string cachedDuplicate;

		private void OnEnable()
		{
			titleContent = new GUIContent(windowTitle);
			usageFoldout = EditorPrefs.GetBool(USAGE_PREFS_KEY, true);

			Selection.selectionChanged += OnSelectionChanged;
			EditorApplication.hierarchyChanged += OnSelectionChanged;
			onLanguageChanged += OnLanguageChanged;

			OnSelectionChanged();
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= OnSelectionChanged;
			EditorApplication.hierarchyChanged -= OnSelectionChanged;
			onLanguageChanged -= OnLanguageChanged;
		}

		[MenuItem("Tools/" + PRODUCT_NAME, false, 1000)]
		internal static void Open()
		{
			var window = GetWindow<HierarchyPathWindow>();
			window.minSize = new Vector2(420f, 150f);
			window.Show();
		}

		// 선택 그대로 두고 이름만 고치면 경고가 안 바뀌어서 hierarchyChanged 도 같이 물림
		private void OnSelectionChanged()
		{
			var t = Selection.activeTransform;
			if (t) lastTarget = t;

			cachedDuplicate = lastTarget
				? PathBuilder.FindDuplicateName(lastTarget, PathBuilder.GetAvatarRoot(lastTarget))
				: null;

			Repaint();
		}

		private void OnLanguageChanged()
		{
			titleContent = new GUIContent(windowTitle);
			Repaint();
		}

		private void OnGUI()
		{
			DrawToolbar();

			scroll = EditorGUILayout.BeginScrollView(scroll);
			DrawSelection();
			DrawUsage();
			EditorGUILayout.EndScrollView();
		}

		private void DrawToolbar()
		{
			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				GUILayout.FlexibleSpace();

				EditorGUI.BeginChangeCheck();
				var picked = (Language)EditorGUILayout.Popup(
					(int)language, languageNames, EditorStyles.toolbarPopup, GUILayout.Width(90f));

				if (EditorGUI.EndChangeCheck()) language = picked;
			}
		}

		private void DrawSelection()
		{
			EditorGUILayout.Space(4f);
			EditorGUILayout.LabelField(selectionHeader, EditorStyles.boldLabel);

			// 딴 데 클릭했다고 경로 지워지면 복사를 못 함
			var target = lastTarget;
			if (!target)
			{
				EditorGUILayout.LabelField(nothingSelected, EditorStyles.miniLabel);
				return;
			}

			string basis, problem;
			var absolute = PathBuilder.GetAbsolutePath(target);
			var relative = PathBuilder.GetRelativePath(target, out basis, out problem);
			var scene = PathBuilder.GetScenePath(target);


			var relativeHeader = basis == null ? relativeLabel : RelativeLabelWith(basis);
			var width = MeasureLabelWidth(absoluteLabel, relativeHeader, sceneLabel);

			if (absolute == null) DrawNote(width, absoluteLabel, noAvatarRoot);
			else DrawPath(width, absoluteLabel, absolute);

			if (relative == null) DrawNote(width, relativeHeader, problem);
			else DrawPath(width, relativeHeader, relative);

			DrawPath(width, sceneLabel, scene);

			EditorGUILayout.Space(4f);

			if (Selection.transforms.Length > 1)
				EditorGUILayout.LabelField(ManySelected(Selection.transforms.Length), EditorStyles.miniLabel);

			if (cachedDuplicate != null)
				EditorGUILayout.HelpBox(DuplicateName(cachedDuplicate), MessageType.Warning);
		}

		private void DrawPath(float width, string label, string path)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(new GUIContent(label, label), GUILayout.Width(width));

				EditorGUILayout.SelectableLabel(path, EditorStyles.textField,
				   GUILayout.Height(EditorGUIUtility.singleLineHeight));

				using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(path)))
				{
					if (GUILayout.Button(copyButton, GUILayout.Width(50f)))
						EditorGUIUtility.systemCopyBuffer = path;
				}
			}
		}

		private void DrawNote(float width, string label, string message)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(new GUIContent(label, label), GUILayout.Width(width));
				EditorGUILayout.LabelField(message, EditorStyles.miniLabel);
			}
		}

		private void DrawUsage()
		{
			EditorGUILayout.Space(10f);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1f), new Color(0f, 0f, 0f, 0.2f));
			EditorGUILayout.Space(4f);

			EditorGUI.BeginChangeCheck();
			usageFoldout = EditorGUILayout.Foldout(usageFoldout, usageHeader, true);
			if (EditorGUI.EndChangeCheck()) EditorPrefs.SetBool(USAGE_PREFS_KEY, usageFoldout);

			if (!usageFoldout) return;

			// EditorStyles 직접 고치면 다른 창까지 바뀜. 복사본 사용
			if (usageStyle == null)
				usageStyle = new GUIStyle(EditorStyles.wordWrappedLabel) { fontSize = USAGE_FONT_SIZE };

			using (new EditorGUI.IndentLevelScope())
			{
				foreach (var line in usageLines)
				{
					EditorGUILayout.LabelField("• " + line, usageStyle);
					EditorGUILayout.Space(3f);
				}
			}
		}

		private float MeasureLabelWidth(params string[] labels)
		{
			var width = MIN_LABEL_WIDTH;
			foreach (var label in labels)
				width = Mathf.Max(width, EditorStyles.label.CalcSize(new GUIContent(label)).x);

			var cap = Mathf.Max(MIN_LABEL_WIDTH, position.width - PATH_COLUMN_ROOM);
			return Mathf.Min(width + 8f, cap);
		}

	}
}
