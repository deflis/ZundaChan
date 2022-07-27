# ZundaChan

[VOICEVOX ENGINE](https://github.com/VOICEVOX/voicevox_engine) の棒読みちゃん互換クライアントです。
現在は棒読みちゃんの IpcClientChannel 接続のみをサポートしています。
IpcClientChannel 接続によって、 [マルチコメントビューア](https://ryu-s.github.io/app/multicommentviewer) などで VOICEVOX による読み上げができます。

## 動作を確認した棒読みちゃん連携クライアント

- [マルチコメントビューア](https://ryu-s.github.io/app/multicommentviewer)

## 使い方

[https://github.com/deflis/ZundaChan/releases] からダウンロードできます。

### ZundaChan の設定方法

同じディレクトリにある `ZundaChan.toml` を編集してください。TOML 形式になっています。（本来は実行ファイル名に対応した toml ファイルになっています）

起動時に以下の表示が出ます。これらは設定ファイルで利用可能な設定項目になります。現在選択されているものには `*` がついています

```
AudioDevices:
-1:*Microsoft Sound Mapper
0: VoiceMeeter Input (VB-Audio Voi
1: スピーカー (Voidol 音声)
...(略)
Speakers:
2: 四国めたん(ノーマル)
0: 四国めたん(あまあま)
...(略)
```

- `AudioDevices` が音声デバイスの指定 `device` の ID となっています。デフォルトは `-1` です。
- `Speakers` がキャラクターの指定 `speaker` の ID となっています。デフォルトは `3` (ずんだもん(ノーマル))です。
- `voicevox_engine` は、VOICEVOX ENGINE への接続先を示します。互換性のある COEIROLINK 等でも利用可能だと思いますが現時点では未確認です。

### 起動後の設定リロード及び終了

- r キーを押すことにより、起動中でも設定の再読み込みをすることができます。
- q キーを押すことにより、終了します。

### マルチコメントビューアへの設定

棒読みちゃん実行ファイルの代わりに指定することができます。選択ダイアログで `*` をファイル名のところに入れると `BouyomiChan.exe` 以外でも指定可能です。
なお、 `BouyomiChan.exe` にファイル名を変更した場合は設定ファイルも同じ名前に更新してください。

## 他の方法との比較

同じ用途で主に利用可能な手法は以下の 2 つです。それぞれについて比較を行います。

- [棒読みちゃん](https://chi.usamimi.info/Program/Application/BouyomiChan/) + [SAPI For VOICEVOX](https://github.com/shigobu/SAPIForVOICEVOX)
- [偽装ちゃん](https://hgotoh.jp/wiki/doku.php/documents/tools/tools-206) + [AssistantSeika](https://hgotoh.jp/wiki/doku.php/documents/voiceroid/assistantseika/assistantseika-000)

### 棒読みちゃん + SAPI For VOICEVOX

直接コメントビューアや VOICEVOX に接続するため、SAPI のようにシステムに登録する必要がなくなります。
また、AquesTalk を利用しているため、公開終了になるリスクがあります。

複数のアプリケーションの組み合わせで利用するため、万が一トラブルが発生した時に切り分けが難しくなる可能性があります。
コメビュ側からの話者の切り替えも難しいです。

### 偽装ちゃん + AssistantSeika

話者マップのような機能がないシンプルな構造のため、話者の追加に即座に対応可能です。
また、棒読みちゃん + SAPI と同じく複数のアプリケーションの組み合わせで利用するため、万が一トラブルが発生した時に切り分けが難しくなる可能性があります。

## 未実装の機能

- 音量調整機能
  - Naudio ライブラリによって実装可能になっているはずです。今後の課題です。
- メッセージの加工機能
- IPC 接続での以下の機能
  - 話者指定機能
  - 速度等の変更機能
  - ワーカーキュー管理機能
- Socket 接続
  - 未実装
- HTTP 接続
  - 未実装
  - 開発者の PC が Hyper-V 環境のため、利用できない可能性がありかなり後回しになる可能性が高いです。

## 参考にしました

- [棒読みちゃん](https://chi.usamimi.info/Program/Application/BouyomiChan/)
- [偽装ちゃん](https://hgotoh.jp/wiki/doku.php/documents/tools/tools-206)
  - 偽装ちゃんの実装より着想を得て IPC 接続を実装しました。この場を借りてお礼申し上げます。

## スペシャルサンクス

- [ヒホさん](https://twitter.com/hiho_karuta)
- VOICEVOX 開発者の皆様
