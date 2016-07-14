using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ターゲットの位置に瞬間移動するクラス
/// </summary>

public class Teleportation : MonoBehaviour
{

    [SerializeField] GameObject ownPlayer;
    [SerializeField] GameObject targetPointer;
    SteamVR_TrackedObject trackedObj;
    Vector3 movePos;

    void Start()
    {
        //任意のViveコントローラーから取得
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        //ターゲットポインタを隠す
        targetPointer.SetActive(false);
        //デバイスの入力受け付け
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        //Updateとして購読
        this.UpdateAsObservable()
        //コントローラーのトリガーを押している間発火
            .Where(_ => device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        //ターゲットポインタを表示
            .Subscribe(_ => targetPointer.SetActive(true));

        this.UpdateAsObservable()
        //コントローラーのトリガーを離した時発火
            .Where(_ => device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        //ターゲットポインタの位置へ移動
            .Subscribe(_ => moveToPoint());
    }

    /// <summary>
    /// ポイントへ移動
    /// </summary>
    void moveToPoint()
    {
        //ターゲットポインタの位置を代入
        movePos = targetPointer.transform.position;
        //Playerが地面に食い込まないように調整
        movePos.y = 1;
        //Playerを調整した位置へ移動
        ownPlayer.transform.position = movePos;
        //ターゲットポインタを隠す
        targetPointer.SetActive(false);
    }
}
