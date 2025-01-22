# OllamaSharpTest

# ■言語
C#

# ■事前準備
## Ollama
1. Ollama (https://ollama.com/download) のインストーラーをリンクからダウンロード及びインストールする。
2. コマンドプロンプトまたはPowerShellから `ollama pull phi3:latest` を実行する。
3. `ollama serve` を実行してローカルサーバーを立ち上げる。

## whisper.net
1. Hugging Face (https://huggingface.co/ggerganov/whisper.cpp/tree/main) にアクセスする。
2. small、base、medium、large の順に音声認識の精度が上がる。適当なモデルデータをダウンロードする。
3. 実行ファイルが読み込むディレクトリにbinファイルを置く。
