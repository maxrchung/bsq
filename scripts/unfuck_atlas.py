#!/usr/bin/env -S uv run --script

import sys

from PIL import Image

CARD_WIDTH = 125
CARD_HEIGHT = 175


def convert(in_path: str, in_path_2: str, out_path: str):
    img = Image.open(in_path)
    new_size = (img.width // 2, img.height // 2)
    img = img.resize(new_size, resample=Image.Resampling.NEAREST)
    img2 = Image.open(in_path_2)
    new_size = (img2.width // 2, img2.height // 2)
    img2 = img2.resize(new_size, resample=Image.Resampling.NEAREST)
    out_atlas = Image.new("RGBA", (2048, 2048))

    def copy_cards(start_idx: int, count: int, row: int, offset: int = 0):
        end_idx = start_idx + count
        orig_rect = (start_idx * CARD_WIDTH, 0, end_idx * CARD_WIDTH, CARD_HEIGHT)
        orig = img.crop(orig_rect)
        out_rect = (offset * CARD_WIDTH, row * CARD_HEIGHT)
        out_atlas.paste(orig, out_rect)

    copy_cards(2 + (13 * 0), 13, 0) # spades
    copy_cards(2 + (13 * 1), 13, 1) # hearts
    copy_cards(2 + (13 * 2), 13, 2) # clubs
    copy_cards(2 + (13 * 3), 13, 3) # diamonds
    out_atlas.paste(img2, (0, 4 * CARD_HEIGHT)) # faces
    copy_cards(2 + (13 * 4), 10, 5, 2) # misc
    copy_cards(0, 2, 5, 0) # front and back

    out_atlas.save(out_path)


if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Usage: unfuck_atlas.py <input_path> <extras_path> <output_path>")
        sys.exit(1)
    convert(sys.argv[1], sys.argv[2], sys.argv[3])
