import codecs
import chardet
import os
import fitz
import pandas as pd
from pathlib import Path
from docx import Document

src_directory = '../convert/'
dest_directory = '../convert/utf8/'

files = os.listdir('../convert')

for i, file_name in enumerate(files):
    src_file = os.path.join(src_directory, file_name)
    dest_file = os.path.join(dest_directory, os.path.basename(Path(file_name).stem + ".txt"))

    # For checking file extension
    extension = Path(src_file).suffix.lower()

    # Skip if it's a folder
    if os.path.isdir(src_file):
        continue

    try:
        os.makedirs(dest_directory, exist_ok = True)

        with open(src_file, 'rb') as f:
            # Check the encoding of the opened file
            raw_data = f.read()
            detected = chardet.detect(raw_data)
            detected_encoding = detected['encoding']
        
        if extension == ".pdf":
            doc = fitz.open(src_file) 
            with open(dest_file, "w", encoding = "utf-8") as uf:
                for text in doc:
                    uf.write(text.get_text() + "\n\n")
                    
        elif extension in [".docx", ".doc"]:
            doc = Document(src_file)
            with open(dest_file, "w", encoding = "utf-8") as uf:
                text = "\n".join([para.text for para in doc.paragraphs])
                uf.write(text)

        elif extension in [".xlsx", ".xls"]:
            if extension == ".xls":
                engine = "xlrd"
            elif extension == ".xlsx":
                engine = "openpyxl"

            df = pd.read_excel(src_file, sheet_name = None, engine = engine)           
            with open(dest_file, "w", encoding = "utf-8") as uf:
                text = "\n".join(["\n".join(map(str, df[sheet].values.flatten())) for sheet in df])
                uf.write(text)
                
        else:
            # Open with detected encoding
            with codecs.open(src_file, 'r', encoding = detected_encoding) as sf:
                
                # Open the output file
                with codecs.open(dest_file, 'w', encoding = 'utf-8') as uf:        
                    for text in sf:
                        uf.write(text)

        print(f'Successfully processed: {file_name}')

    except Exception as e:
        print(f'Error processing {file_name}: {e}')
