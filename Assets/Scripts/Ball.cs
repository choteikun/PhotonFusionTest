using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//如果實體子彈過快，可能要嘗試用射線檢測
public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }//Tick計時器

    [SerializeField]
    private float bulletSpeed = 5f;

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);//(可以在任何有NetworkBehavior的地方下調用Runner)倒數5秒，這裡的秒數指的是創建一個持續時間為5秒的計時器實例，存儲在life變量中。計時器創建後，可以使用它來執行特定的操作，例如在計時器結束時銷毀對像或執行其他遊戲邏輯。
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;
        }

        DetectCollision();

    }

    private void DetectCollision()//fusion官方不推薦使用unity的 OnTriggerEnter & OnTriggerCollision做網路上的物裡碰撞，是因為Fusion網路狀態的更新率和Unity物理引擎的更新率不相同，而且無法做客戶端預測
    {
        if (Object = null) return;//檢測網路物件是否為空
        if (!Object.HasStateAuthority) return;//只會在伺服器端做檢測

        var colliders = Physics.OverlapSphere(transform.position, radius: 175f);//畫一顆球，並檢測球裡的所有collider並回傳

        foreach(var collider in colliders)
        {
            if(collider.TryGetComponent<PlayerController>(out PlayerController playerController))//判斷collider身上是否有PlayerController的腳本
            {
                playerController.TakeDamage(10);

                Runner.Despawn(Object);
            }
            else
            {
                // 沒有找到組件
                // 做一些錯誤處理
            }
        }
    }

}