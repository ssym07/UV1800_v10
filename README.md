# UV1800 Automated Measurement System

## Overview

本プロジェクトは、UV-1800分光光度計を用いた長時間吸光測定を自動化するために開発したVB.NET製制御ソフトウェアです。
従来は半日以上に及ぶ測定を手動で監視・操作する必要があり、人的負荷や操作ミスが課題となっていました。
本ソフトウェアでは、測定シーケンスの自動実行、時間間隔制御、ログ保存を実装し、研究作業の効率化と再現性向上を実現しています。

---

## Features

* 指定時間間隔での自動測定
* 長時間連続運転への対応
* セッションログ自動保存
* エラー発生時の停止処理
* GUIによる簡易操作

---

## Development Environment

* VB.NET
* .NET Framework 4.7.2
* Visual Studio
* Windows

---

## Motivation

研究活動において、長時間測定中は常時監視が必要であり、作業負荷が大きいという課題がありました。
そこで、測定フローをソフトウェア化し、自動制御による省力化と測定安定性向上を目的として開発しました。

---

## System Structure

```text
MainForm
 ├ 測定開始・停止
 ├ UI操作
 └ 測定条件入力

Module1
 ├ 測定制御
 ├ 時間管理
 ├ ログ出力
 └ 装置通信
```

---

## Directory Structure

```text
UV1800_v10/
├ src/
├ docs/
├ screenshots/
├ logs/
├ README.md
└ .gitignore
```

---

## Screenshots

※ 必要に応じてUI画面や測定中画面を追加予定

---

## Future Improvements

* 非同期処理によるUI安定化
* エラー処理の高度化
* 設定ファイル外部化
* リアルタイムグラフ表示
* データ解析機能追加

---

## License

This project is for academic and research purposes.
