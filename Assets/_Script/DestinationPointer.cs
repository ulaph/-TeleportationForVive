using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 放物線を表示するクラス
/// ラインレンダラーの点の数と
/// 初速を自由に設定して使用
/// </summary>

public class DestinationPointer : MonoBehaviour
{

    [SerializeField] GameObject targetMarker;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float vertexCount = 25;
    [SerializeField] float initialVelocity = 1;
    List<Vector3> vertexs = new List<Vector3>();
    static readonly float Gravity = 9.81F;

    void Start()
    {
        //コントローラの入力の後に読みたい
        this.LateUpdateAsObservable()
        //ターゲットポインタが表示されている時イベントを発火
            .Where(_ => targetMarker.activeSelf)
        //放物線を表示させる
            .Subscribe(_ =>
            {
                lineRenderer.enabled = true;
                setOrbitState();
            });


        this.LateUpdateAsObservable()
        //ターゲットポインタが非表示の時イベントを発火
            .Where(_ => !targetMarker.activeSelf)
        //放物線を非表示にする
            .Subscribe(_ => lineRenderer.enabled = false);
    }

    /// <summary>
    /// 放物線を表示する関数
    /// </summary>
    void setOrbitState()
    {
        //コントローラの向いている角度(x軸回転)をラジアン角へ
        var angleFacing = -Mathf.Deg2Rad * transform.eulerAngles.x;
        var h = transform.position.y;
        var v0 = initialVelocity;
        var sin = Mathf.Sin(angleFacing);
        var cos = Mathf.Cos(angleFacing);
        var g = Gravity;
        //地面に到達する時間
        //t = (v0 * sinθ) / g + √ (v0^2 * sinθ^2) / g^2 + 2 * h / g
        var arrivalTime = (v0 * sin) / g + Mathf.Sqrt((square(v0) * square(sin)) / square(g) + (2F * h) / g);

        for (var i = 0; i < deltaLinePoint; i++)
        {
            //delta時間あたりのワールド座標(ラインレンダラーの節)
            var delta = i * arrivalTime / deltaLinePoint;
            var x = v0 * cos * delta;
            var y = v0 * sin * delta - 0.5F * g * square(delta);
            //コントローラのx,z平面のベクトル
            var forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            var point = transform.position + forward * x + Vector3.up * y;
            //Listにpointの座標を追加
            pointList.Add(point);
        }
        //LineRendererの頂点数
        lineRenderer.SetVertexCount(pointList.Count);
        //ターゲットポインタをポイントの最終地点に設置
        targetMarker.transform.position = pointList.Last();
        //LineRendererのPointsに設置
        lineRenderer.SetPositions(pointList.ToArray());
        //リストの初期化
        pointList.Clear();
    }

    /// <summary>
    /// 引数の2乗を返す関数
    /// </summary>
    /// <param name="num">Number.</param>
    static float square(float num)
    {
        return Mathf.Pow(num, 2);
    }
}
