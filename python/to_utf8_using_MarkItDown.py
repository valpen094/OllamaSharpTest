import chardet
import os
from pathlib import Path
from markitdown import MarkItDown 
import sys
import io

src_directory = '../convert/'
tmp_directory = '../convert/tmp/'
dest_directory = '../convert/utf8/'

files = os.listdir('../convert')

for i, file_name in enumerate(files):
    src_file = os.path.join(src_directory, file_name)
    tmp_file = os.path.join(tmp_directory, os.path.basename(Path(file_name).stem + ".txt"))
    dest_file = os.path.join(dest_directory, os.path.basename(Path(file_name).stem + ".txt"))

    # For checking file extension
    extension = Path(src_file).suffix.lower()

    # Skip if it's a folder
    if os.path.isdir(src_file):
        continue

    try:
        os.makedirs(dest_directory, exist_ok = True)
        os.makedirs(tmp_directory, exist_ok = True)

        markitdown = MarkItDown()
        result = markitdown.convert(src_file) # Convert to MarkDown format

        # Output to StringIO
        output = io.StringIO()
        sys.stdout = output
        print(result.text_content)
        sys.stdout = sys.__stdout__

        # Assign the content that would have been output to standard output
        result = output.getvalue()

        with open(src_file, 'rb') as f:
            # Check the encoding of the opened file
            raw_data = f.read()
            detected = chardet.detect(raw_data)
            detected_encoding = detected['encoding']

        # Save to a temporary folder
        with open(tmp_file, 'w', encoding = detected_encoding) as tf:    
            tf.write(result)

        # Open with detected encoding
        with open(tmp_file, 'r', encoding = detected_encoding) as sf:

            # Open the output file      
            with open(dest_file, "w", encoding = "utf-8") as uf:
                for text in sf:
                    uf.write(text)

            print(f'Successfully processed: {file_name}')

    except Exception as e:
        print(f'Error processing {file_name}: {e}')
