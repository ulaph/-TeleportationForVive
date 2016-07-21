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
    [SerializeField] GameObject targetMarker;
    SteamVR_TrackedObject trackedObj;

    void Start()
    {
        targetMarker.SetActive(false);
        //任意のViveコントローラーから取得
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        //デバイスの入力受け付け
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        //Updateとして購読
        this.UpdateAsObservable()
            .Where(_ => device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) //コントローラーのトリガーを押している間イベントを発火
            .Subscribe(_ => targetMarker.SetActive(true));

        this.UpdateAsObservable()
            .Where(_ => device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) //コントローラーのトリガーを離した時イベントを発火
            .Subscribe(_ => moveToPoint()); //ターゲットマーカーの位置へ移動
    }

    /// <summary>
    /// 移動先の位置を調整して移動
    /// </summary>
    void moveToPoint()
    {
        ownPlayer.transform.position = targetMarker.transform.position;
        targetMarker.SetActive(false);
    }
}
