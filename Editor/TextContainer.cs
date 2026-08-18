using System;
using UnityEditor;
using UnityEngine;

namespace HareStudio.HierarchyPath
{
	internal static class TextContainer
	{
		private const string LANGUAGE_PREFS_KEY = "HierarchyPath.Language";

		internal enum Language { English, Japanese, Korean }

		internal static readonly string[] languageNames = { "English", "日本語", "한국어" };

		internal static event Action onLanguageChanged;

		private static Language currentLanguage;
		private static bool languageLoaded;

		internal static Language language
		{
			get
			{
				if (!languageLoaded)
				{
					currentLanguage = (Language)EditorPrefs.GetInt(LANGUAGE_PREFS_KEY, (int)DetectEditorLanguage());
					languageLoaded = true;
				}

				return currentLanguage;
			}
			set
			{
				if (languageLoaded && currentLanguage == value) return;

				currentLanguage = value;
				languageLoaded = true;
				EditorPrefs.SetInt(LANGUAGE_PREFS_KEY, (int)value);
				onLanguageChanged?.Invoke();
			}
		}

		private static Language DetectEditorLanguage()
		{
			switch (Application.systemLanguage)
			{
				case SystemLanguage.Japanese: return Language.Japanese;
				case SystemLanguage.Korean: return Language.Korean;
				default: return Language.English;
			}
		}

		private static string GetLoc(string en, string ja, string ko)
		{
			switch (language)
			{
				case Language.Japanese: return ja;
				case Language.Korean: return ko;
				default: return en;
			}
		}

		internal static string windowTitle => GetLoc("Hierarchy Path", "階層パス", "계층 경로");
		internal static string selectionHeader => GetLoc("Current Selection", "現在の選択", "현재 선택");
		internal static string nothingSelected => GetLoc("Nothing selected.", "選択されていません。", "선택된 항목이 없습니다.");
		internal static string copyButton => GetLoc("Copy", "コピー", "복사");
		internal static string usageHeader => GetLoc("Usage", "使い方", "사용법");

		internal static string absoluteLabel => GetLoc("Absolute Path (Avatar Root)", "絶対パス (アバタールート)", "절대 경로 (아바타 루트)");
		internal static string sceneLabel => GetLoc("Scene Path", "シーンパス", "씬 경로");
		internal static string relativeLabel => GetLoc("Relative Path", "相対パス", "상대 경로");

		internal static string MergeAnimatorBasis(string name) => "MA: " + name;
		internal static string AnimatorBasis(string name) => "Animator: " + name;

		internal static string RelativeLabelWith(string basis) =>
			GetLoc("Relative Path (" + basis + ")", "相対パス (" + basis + ")", "상대 경로 (" + basis + ")");

		internal static string noAvatarRoot => GetLoc(
			"No Avatar Descriptor or Animator above the selection.",
			"選択オブジェクトの上位に Avatar Descriptor または Animator がありません。",
			"선택 항목 상위에 아바타 디스크립터나 Animator가 없습니다.");

		internal static string noRelativeBasis => GetLoc(
			"No MA Merge Animator or Animator above the selection.",
			"選択オブジェクトの上位に MA Merge Animator または Animator がありません。",
			"선택 항목 상위에 MA Merge Animator 또는 Animator가 없습니다.");

		internal static string PathModeIsAbsolute(string name) => GetLoc(
			"\"" + name + "\" is set to Absolute path mode.",
			"\"" + name + "\" のパスモードが絶対パスです。",
			"\"" + name + "\"의 경로 모드(Path Mode)가 절대 경로입니다.");

		internal static string DuplicateName(string name) => GetLoc(
			"\"" + name + "\" shares its name with a sibling. Unity paths cannot tell them apart, so this path may resolve to the wrong object. Rename one of them.",
			"同じ親の下に \"" + name + "\" という名前のオブジェクトが複数あります。Unity のパスでは区別できず、別のオブジェクトを指してしまう可能性があるため、いずれかをリネームしてください。",
			"같은 부모 아래에 \"" + name + "\" 이름의 오브젝트가 둘 이상 있습니다. Unity 경로로는 구분할 수 없어 엉뚱한 오브젝트에 연결될 수 있으니 하나는 이름을 바꾸세요.");

		internal static string CopiedPaths(int count, string paths) => GetLoc(
			"Copied " + count + " path(s):\n" + paths,
			count + " 件のパスをコピーしました:\n" + paths,
			"경로 " + count + "개를 복사했습니다:\n" + paths);

		internal static string ManySelected(int count) => GetLoc(
			count + " selected; showing the active one. Right-click in the Hierarchy to copy them all.",
			count + " 件選択中。アクティブなもののみ表示しています。全件コピーは Hierarchy の右クリックから。",
			count + "개 선택됨 - 활성 항목만 표시. 전체 복사는 Hierarchy 우클릭으로.");

		internal static string[] usageLines => new[]
		{
			GetLoc("You can also copy from the Hierarchy right-click menu, under Hierarchy Path.",
				"Hierarchy の右クリックメニュー内 Hierarchy Path からもコピーできます。",
				"Hierarchy 우클릭 메뉴의 Hierarchy Path에서도 복사할 수 있습니다."),

			GetLoc("Select several objects and right-click to copy one path per line.",
				"複数選択して右クリックすると、1 行に 1 つずつコピーされます。",
				"여러 개를 선택하고 우클릭하면 한 줄에 하나씩 복사됩니다."),

			GetLoc("The relative path uses an MA Merge Animator set to Relative mode. With none present, it falls back to the nearest Animator below the avatar root.",
				"相対パスは、相対モードの MA Merge Animator を基準にします。ない場合は、アバタールートより下にある最も近い Animator を使います。",
				"상대 경로는 모듈러 아바타의 MA Merge Animator가 상대 경로 모드일 때 그것을 기준으로 합니다. 없으면 아바타 루트보다 아래에 있는 가장 가까운 Animator를 사용합니다."),

			GetLoc("Unity paths are name-based, so two siblings with the same name cannot be told apart. Rename one when warned.",
				"Unity のパスは名前だけで辿るため、同じ親の下に同名のオブジェクトがあると区別できません。警告が出たらどちらかをリネームしてください。",
				"Unity 경로는 이름만으로 찾아가기 때문에, 같은 부모 아래에 이름이 같은 오브젝트가 있으면 구분되지 않습니다. 경고가 뜨면 하나는 이름을 바꾸세요."),

			GetLoc("To use shortcuts, assign them under Edit > Shortcuts > Hierarchy Path.",
				"ショートカットを使うには、Edit > Shortcuts > Hierarchy Path から設定してください。",
				"단축키를 사용하려면 Edit > Shortcuts > Hierarchy Path에서 설정하세요."),
		};
	}
}
