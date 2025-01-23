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
2. 適当なモデルデータをダウンロード。small、base、medium、large の順に音声認識の精度が上がる。
3. 実行ファイルが読み込むディレクトリにbinファイルを置く。

## chromadb
1. `pip install chromadb` を実行する。
2. `chroma.exe run --path "Path" --host localhost --port 8000` を実行してサーバーを起動する。"Path"の部分はデータベースを構築したいディレクトリを指定する。例えばCドライブに"Database"という名前で構築したい場合、"C:\Database"と指定すること。
3. 上記実行後、コンソールに以下のように出力されればOK。
   ![image](https://github.com/user-attachments/assets/dfe868ba-0b94-4601-9c8e-d3ad04b26220)

# ■データセット
- e-Stat (https://www.e-stat.go.jp/stat-search/files?page=1&query=csv&layout=dataset)
- 法人番号公表サイト (https://www.houjin-bangou.nta.go.jp/download/zenken/index.html#csv-sjis)
- 東京都オープンデータカタログサイト (https://catalog.data.metro.tokyo.lg.jp/dataset)
