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

public class Parabola : MonoBehaviour
{

    [SerializeField] GameObject targetPointer;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float linePointInterval = 25;
    [SerializeField] float initialVelocity = 1;
    List<Vector3> pointList = new List<Vector3>();
    float gravity = 9.81F;

    void Start()
    {
        //ターゲットポインタの状態が更新された後に読みたい
        this.LateUpdateAsObservable()
        //ターゲットポインタが表示されている時発火
            .Where(_ => targetPointer.activeSelf)
        //放物線を表示させる
            .Subscribe(_ =>
            {
                lineRenderer.enabled = true;
                showOrbit();
            });


        this.LateUpdateAsObservable()
        //ターゲットポインタが非表示の時発火
            .Where(_ => !targetPointer.activeSelf)
        //放物線を非表示にする
            .Subscribe(_ => lineRenderer.enabled = false);
    }

    /// <summary>
    /// 放物線を表示する関数
    /// </summary>
    void showOrbit()
    {
        //コントローラの向いている角度(x軸回転)
        var angleFacing = -Mathf.Deg2Rad * transform.eulerAngles.x;
        //コントローラの地面からの高さ
        var h = transform.position.y;
        //初速
        var v0 = initialVelocity;
        //v0の正弦，余弦成分
        var sin = Mathf.Sin(angleFacing);
        var cos = Mathf.Cos(angleFacing);
        //重力加速度
        var g = gravity;
        //地面に到達する時間
        var arrivalTime = (v0 * sin) / g + Mathf.Sqrt((square(v0) * square(sin)) / square(g) + (2F * h) / g);
       
        for (float i = 0; i <= arrivalTime; i += arrivalTime / linePointInterval)
        {
            //i時間あたりの座標
            //x座標
            var x = v0 * cos * i;
            //y座標
            var y = v0 * sin * i - 0.5F * g * square(i);
            //コントローラーのx,z平面のベクトル
            var forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            //コントローラの位置からポイントの位置を決定
            var point = transform.position + forward * x + Vector3.up * y;
            //Listにpointの座標を追加
            pointList.Add(point);
        }
        //LineRendererの頂点数
        lineRenderer.SetVertexCount(pointList.Count);
        //ターゲットポインタをポイントの最終地点に設置
        targetPointer.transform.position = pointList.Last();
        //LineRendererに設置
        lineRenderer.SetPositions(pointList.ToArray());
        //リストの初期化
        pointList.Clear();
    }

    /// <summary>
    /// 引数の2乗を返す関数
    /// </summary>
    /// <param name="num">Number.</param>
    float square(float num)
    {
        return Mathf.Pow(num, 2);
    }
}
