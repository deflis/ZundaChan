# ZundaChan

[VOICEVOX ENGINE](https://github.com/VOICEVOX/voicevox_engine) の棒読みちゃん互換クライアントです。
現在は棒読みちゃんの IpcClientChannel 接続のみをサポートしています。
IpcClientChannel 接続によって、 [マルチコメントビューア](https://ryu-s.github.io/app/multicommentviewer) などで VOICEVOX による読み上げができます。

## 動作を確認した棒読みちゃん連携クライアント

- [マルチコメントビューア](https://ryu-s.github.io/app/multicommentviewer)

## 使い方

後で書きます。

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
