using System;
using Common;
using Controller;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using GridType = Manager.GridType;

namespace Scene
{
    public class Main : UnitySingleton<Main>
    {
        public PlayerController p1;
        public PlayerController p2;
        
        private void Start()
        {
            SoundManager.Instance.PlayBGM(SoundManager.AudioSo.bgm);
            p1 = Global.P1;
            p2 = Global.P2;
        }


        public void Restart()
        {
            p1.ResetPos();
            p2.ResetPos();
            p1.Builder.BridgeCount = p1.Builder.startBridgeCount;
            p2.Builder.BridgeCount = p2.Builder.startBridgeCount;
            
            GridManager.Instance.DestroyAll(GridType.Bridge);
        }

        private void Update()
        {
            if (!WinUI.Instance.toggle.isShowing && ((Vector2) p1.transform.position - (Vector2) p2.transform.position).magnitude < 1e-4)
            {
                WinUI.Instance.toggle.Show(true);
            }
        }

        public void Exit()
        {
            SceneManager.LoadScene("Start");
        }
    }
}