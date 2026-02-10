using System.IO;
using UnityEngine;

namespace MyAssets
{
    // JSONデータの読み書きを行うマネージャークラス
    // SettingJSONData型のデータを保存・読み込みする
    //
    public class DataManager
    {
        // 外部から直接SettingDataへアクセスして変更できるようにプロパティを公開
        public static SettingJSONData   SettingData { get; set; }
        private static string           mSettingFilePath;
        private static string           mSettingFileName = "settingData.json";
        // JSONデータの初期化
        public static void SettingDataAwake()
        {
            // パス名取得
            mSettingFilePath = Path.Combine(Application.persistentDataPath, mSettingFileName);
            // ファイルがないとき、ファイル作成
            if (!File.Exists(mSettingFilePath))
            {
                SettingData = new SettingJSONData();
                Save(SettingData);
                Debug.Log("設定ファイルを新規作成しました: " + mSettingFilePath);
            }
            else
            {
                // ファイルを読み込んで格納
                SettingData = Load(mSettingFilePath);
                Debug.Log("設定ファイルを読み込みました: " + mSettingFilePath);
            }
        }
        // jsonファイル保存
        public static void Save(SettingJSONData data)
        {
            //念のためnullチェック
            if (data == null) return; 
            string json = JsonUtility.ToJson(data);                 // jsonとして変換
            StreamWriter wr = new StreamWriter(mSettingFilePath, false);    // ファイル書き込み指定
            wr.WriteLine(json);                                     // json変換した情報を書き込み
            wr.Close();                                             // ファイル閉じる
        }

        // jsonファイル読み込み
        public static SettingJSONData Load(string path)
        {
            StreamReader rd = new StreamReader(path);               // ファイル読み込み指定
            string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
            rd.Close();                                             // ファイル閉じる

            return JsonUtility.FromJson<SettingJSONData>(json);            // jsonファイルを型に戻して返す
        }
    }
}
