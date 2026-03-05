Shader "Custom/ObjectURPShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _RampTex("Ramp Texture (R)",2D) = "gray"{}//トゥーンの階調用
        _OutlineWidth("Outline Width", Range(0.0, 0.05)) = 0.01
    }

    SubShader
    {
        // URPのパス情報。URPにこのSubShaderを使用することを伝える
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        Pass
        {
            // パス名を設定。URPの標準的な描画パス
            Name "Lit"

            HLSLPROGRAM
            // ----------------------------------------------------
            // 1. URPの標準的なインクルードと設定
            // ----------------------------------------------------
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            // URPのコア機能とライティング定義をインクルード
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // ----------------------------------------------------
            // 2. プロパティと構造体の定義
            // ----------------------------------------------------
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_RampTex);
            SAMPLER(sampler_RampTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _MainTex_ST; // Tiling/Offsetの値はCBUFFER内に残す
                float _OutlineWidth;
            CBUFFER_END
            // アプリケーションから頂点シェーダーへ渡されるデータ
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };
            // 頂点シェーダーからフラグメントシェーダーへ渡されるデータ
            struct Varyings
            {
                float4 positionCS : SV_POSITION;// クリップ空間座標
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;// ワールド空間法線
                float3 positionWS : TEXCOORD2;// ワールド空間座標
            };

            //----------------------------------------------------
            // 3. 頂点シェーダー
            //----------------------------------------------------
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                //オブジェクト空間=> ワールド空間への変換
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS = vertexInput.positionWS;

                //頂点のクリップ空間座標を計算
                output.positionCS = TransformWorldToHClip(output.positionWS);

                //法線のワールド空間への変換
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            // ----------------------------------------------------
            // 4. フラグメントシェーダー (frag)
            // ----------------------------------------------------
            half4 frag(Varyings input) : SV_Target
            {
                //ワールド空間の法線と位置を正規化
                float3 normalWS = normalize(input.normalWS);
                
                //メインテクスチャの色を取得
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _BaseColor;

                //現在のメインのライト情報を取得(URPのLighting.hlslから)
                Light mainLight = GetMainLight();

                //N・L(法線とライト方向の内積)を計算
                float NdotL = saturate(dot(normalWS, mainLight.direction));

                //トゥーンシェーディングの核: Ramp Textureを使った階調化
                // NdotLをUVとして使用し、Rampテクスチャから色を取得することで影の境界を決定
                float2 rampUV = float2(NdotL,NdotL);
                float rampValue = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampUV).r;

                // 最終的なディフューズ色 = (ランプテクスチャの値 * ライトの色) * ベース色
                float3 lighting = rampValue * mainLight.color;
                float3 finalColor = lighting * baseColor.rgb;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
