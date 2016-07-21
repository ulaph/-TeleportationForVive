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
            .Where(_ => targetMarker.activeSelf) //ターゲットマーカーが表示されている時イベントを発火
            .Subscribe(_ => //放物線を表示させる
            {
                lineRenderer.enabled = true;
                setOrbitState();
            });


        this.LateUpdateAsObservable()
            .Where(_ => !targetMarker.activeSelf) //ターゲットマーカーが非表示の時イベントを発火
            .Subscribe(_ => lineRenderer.enabled = false); //放物線を非表示にする
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

        for (var i = 0; i < vertexCount; i++)
        {
            //delta時間あたりのワールド座標(ラインレンダラーの節)
            var delta = i * arrivalTime / vertexCount;
            var x = v0 * cos * delta;
            var y = v0 * sin * delta - 0.5F * g * square(delta);
            var forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            var vertex = transform.position + forward * x + Vector3.up * y;
            vertexs.Add(vertex);
        }
        //ターゲットマーカーを頂点の最終地点へ
        targetMarker.transform.position = vertexs.Last();
        //LineRendererの頂点の設置
        lineRenderer.SetVertexCount(vertexs.Count);
        lineRenderer.SetPositions(vertexs.ToArray());
        //リストの初期化
        vertexs.Clear();
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
