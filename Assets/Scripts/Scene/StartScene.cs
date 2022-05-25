using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scene
{
    public class StartScene : UnitySingleton<StartScene>
    {
        private void Awake()
        {
            foreach (var button in FindObjectsOfType<Button>())
            {
                var gameObjectName = button.gameObject.name;
                switch (gameObjectName)
                {
                    case "Start":
                        button.onClick.AddListener(OnStart);
                        break;

                    case "Quit":
                        button.onClick.AddListener(Quit);
                        break;
                }
            }
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit();
#endif
        }

        private void OnStart()
        {
            SceneManager.LoadScene("Main");
        }
    }
}