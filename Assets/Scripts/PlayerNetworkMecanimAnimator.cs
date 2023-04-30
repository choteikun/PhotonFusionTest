using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[OrderAfter(typeof(PlayerController))]
public class PlayerNetworkMecanimAnimator : NetworkBehaviour
{
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }//上一個按鈕的資料

    private PlayerController playerController;
    private NetworkMecanimAnimator networkAnimator;

    //提前Hash進行優化
    readonly int h_Idle = Animator.StringToHash("Idle");
    readonly int h_Run = Animator.StringToHash("Run");
    readonly int h_Sprint = Animator.StringToHash("Sprint");
    readonly int h_Flap = Animator.StringToHash("Flap");
    readonly int h_BeAttack = Animator.StringToHash("BeAttack");
    readonly int h_Die = Animator.StringToHash("Die");




    public override void FixedUpdateNetwork()
	{
		if (IsProxy == true)
			return;

		if (Runner.IsForward == false)
            return;
        //前向模式（Forward Prediction）是指客戶端在處理用戶輸入時，預測將來幾幀內的遊戲狀態，並在本地進行模擬，然後將輸入和狀態同步給服務器。
        //服務器會驗證客戶端的輸入，並進行狀態校正。這種方式可以減少網絡延遲對遊戲體驗的影響，但是需要客戶端做更多的處理，並且容易受到網絡抖動的影響。
        //回放模式（Replay）是指服務器將之前的遊戲狀態記錄下來，並將記錄發送給客戶端，客戶端根據記錄進行本地模擬，並且每隔一定時間向服務器發送用戶輸入。
        //這種方式可以保證客戶端和服務器之間的遊戲狀態一致性，但是會增加網絡延遲和帶寬使用，並且需要記錄和回放遊戲狀態，增加了服務器的負擔。

        if (playerController.GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.Buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);//跟上一個按鈕去做比較
            var released = buttons.GetReleased(ButtonsPrevious);
            ButtonsPrevious = buttons;

            //在State Authority上，建議使用NetworkMecanimAnimator的SetTrigger()方法，以確保觸發器的正確同步。
            //而對於Input Authority，可以使用Animator的原生SetTrigger()方法，因為此時狀態同步不是關鍵問題。

            if (data.Move == Vector3.zero)
            {
                networkAnimator.Animator.SetBool(h_Idle, true);
                networkAnimator.Animator.SetBool(h_Flap, false);
            }
            if (pressed.IsSet(InputButtons.Attack))
            {
                Debug.Log("Flap!!!");
                networkAnimator.Animator.SetBool(h_Flap, true);
                networkAnimator.Animator.SetBool(h_Idle, false);
            }
        }
    }
    protected void Awake()
	{
		playerController = GetComponent<PlayerController>();
		networkAnimator = GetComponent<NetworkMecanimAnimator>();
	}
}
