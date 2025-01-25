import codecs
import chardet
import os

src_directory = '../convert/'
dest_directory = '../convert/utf8/'

files = os.listdir('../convert')

for i, file_name in enumerate(files):
    src_file = os.path.join(src_directory, file_name)
    dest_file = os.path.join(dest_directory, os.path.basename(file_name))

    # Skip if it's a folder
    if os.path.isdir(src_file):
        continue

    try:
        with open(src_file, 'rb') as f:
            # Check the encoding of the opened file
            raw_data = f.read()
            detected = chardet.detect(raw_data)
            detected_encoding = detected['encoding']
        
        # Open with detected encoding
        with codecs.open(src_file, 'r', encoding = detected_encoding) as sf:
            os.makedirs(dest_directory, exist_ok = True)
        
            # Open the output file
            with codecs.open(dest_file, 'w', encoding = 'utf-8') as uf:        
                for line in sf:
                    uf.write(line)

            print(f'Successfully processed: {file_name}')

    except Exception as e:
        print(f'Error processing {file_name}: {e}')
