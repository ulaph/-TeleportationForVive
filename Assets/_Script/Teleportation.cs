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
        targetPointer.SetActive(false);
        //任意のViveコントローラーから取得
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        //デバイスの入力受け付け
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        //Updateとして購読
        this.UpdateAsObservable()
        //コントローラーのトリガーを押している間イベントを発火
            .Where(_ => device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
            .Subscribe(_ => targetPointer.SetActive(true));

        this.UpdateAsObservable()
        //コントローラーのトリガーを離した時イベントを発火
            .Where(_ => device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        //ターゲットポインタの位置へ移動
            .Subscribe(_ => moveToPoint());
    }

    /// <summary>
    /// 移動先の位置を調整して移動
    /// </summary>
    void moveToPoint()
    {
        movePos = targetPointer.transform.position;
        ownPlayer.transform.position = movePos;
        targetPointer.SetActive(false);
    }
}
