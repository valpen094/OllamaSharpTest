# ■OllamaSharpTest
C#製のRAGアプリ。データベースを操作するツールはPythonで作成。<br>以下の機能を実装済み。
- 音声認識
- ローカルLLMとの対話
- 読み上げ
- ベクトルデータベース検索及び情報取得
- RAG
  
# ■言語
- C#
- Python

# ■事前準備
## Ollama
1. Ollama (https://ollama.com/download) のインストーラーをリンクからダウンロード及びインストールする。
2. K/V context cache量子化を有効にするために、ターミナルから以下のコマンドを実行。
- `SETX OLLAMA_FLASH_ATTENTION 1`
- `SETX OLLAMA_KV_CACHE_TYPE "q8_0"`
3. `ollama pull phi3:latest` を実行してモデルをダウンロードする。
4. `ollama serve` を実行してローカルサーバーを立ち上げる。エラーの場合、既にサーバーが立ち上がっているので無視する。
  
## whisper.net
1. Hugging Face (https://huggingface.co/ggerganov/whisper.cpp/tree/main) にアクセスする。
2. 適当なモデルデータをダウンロード。音声認識の精度は、small、base、medium、large の順に高くなっていく。
3. OllamaSharpTestが出力されるディレクトリにbinファイルを置く。
4. 内蔵マイクがない場合、マイクを接続する。

## chromadb
1. `pip install chromadb` を実行する。
2. `chroma.exe run --path "Path" --host localhost --port 8000` を実行してサーバーを起動する。"Path"の部分はデータベースを構築したいディレクトリを指定する。例えばCドライブに"Database"という名前で構築したい場合、"C:\Database"と指定すること。
3. 上記実行後、コンソールに以下のように出力されればOK。
   ![image](https://github.com/user-attachments/assets/dfe868ba-0b94-4601-9c8e-d3ad04b26220)
4. ./python/add_database.py を使うなどしてchromadbにレコードを追加する。

## CUDA Toolkit
Nvidia製グラボを持っており、GPUを使って推論させたい場合はインストールすること。<br>インストール方法などはこちらのサイトを参考にする。https://zenn.dev/yumizz/articles/73d6c7d1085d2f

# ■How To
1. 上記の事前準備を済ませる。
2. OllamaSharpTest.sln をビルドする。
3. OllamaSharpTest.exe を実行する。
4. 自動的にレコーディングが始まるため、質問したいことを話す。<br>
   ![image](https://github.com/user-attachments/assets/192d6112-a578-4b96-a672-644103c6f903)
5. 音声入力したくない場合、Enterを押すとテキスト入力待ちになる。<br>
   ![image](https://github.com/user-attachments/assets/1d1ff1e3-2a1e-4438-b90e-73e95ae12b11)
6. 音声認識またはテキスト入力されるとAIが自動回答する。<br>
   ![image](https://github.com/user-attachments/assets/36fe0b9b-9541-4b6e-8138-12bbe0f4f9b3)
7. 以降はループするので、適当なタイミングで Ctrl + C を押して中断する。
   
# ■ツール
## ./python/add_database.py 
./convert に置かれているファイルをベクトルデータに変換し、chromadbに一括追加する。
1. `pip install ollama`
2. `pip install chromadb`
3. `pip install pandas`
4. `pip install os`
5. `pip install chardet`
   
## ./python/reset_database.py 
./chromadb のデータベース情報をクリアする。ファイル容量は変わらない。

## ./python/query_database.py 
./chromadb のデータベースからベクトル距離が近い情報を抽出する。検索したい内容はスクリプトのuser_textを編集すること。

## ./python/to_utf8.py
./convert に置かれているファイルをutf-8に変換して./convert/utf8 に保存する。
.docx, .doc, .xlsx, .xls, .csv, .txt, .pdfに対応済み。

## ./python/to_utf8_using_MarkItDown.py
./convert に置かれているファイルをマークダウン形式かつutf-8に変換して./convert/utf8 に保存する。（ファイル形式は.txt）<br>to_utf8.pyでサポートしていないファイル形式も対応できるはずだが、テキストの中身によってはエラーになる。

# ■データセット
- e-Stat (https://www.e-stat.go.jp/stat-search/files?page=1&query=csv&layout=dataset)
- 法人番号公表サイト (https://www.houjin-bangou.nta.go.jp/download/zenken/index.html#csv-sjis)
- 東京都オープンデータカタログサイト (https://catalog.data.metro.tokyo.lg.jp/dataset)
