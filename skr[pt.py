import json
import shutil
from pathlib import Path

INPUT_DIR = Path("db")
OUTPUT_DIR = Path("db_converted")

NORMALIZE_LOCALES = False


def normalize_locales(item: dict) -> None:
    if not NORMALIZE_LOCALES:
        return

    locales = item.get("locales")
    if not isinstance(locales, dict):
        return

    for _, data in locales.items():
        if not isinstance(data, dict):
            continue

        if "Name" in data and "name" not in data:
            data["name"] = data.pop("Name")
        if "ShortName" in data and "shortName" not in data:
            data["shortName"] = data.pop("ShortName")
        if "Description" in data and "description" not in data:
            data["description"] = data.pop("Description")


def process_file(input_path: Path, output_path: Path) -> None:
    try:
        with input_path.open("r", encoding="utf-8") as f:
            data = json.load(f)

        if not isinstance(data, dict):
            print(f"[SKIP] {input_path} is not a JSON object")
            return

        new_id = data.get("newId")
        if not new_id or not isinstance(new_id, str):
            print(f"[SKIP] {input_path} missing valid newId")
            return

        converted = dict(data)
        converted.pop("newId", None)

        normalize_locales(converted)

        wrapped = {
            new_id: converted
        }

        output_path.parent.mkdir(parents=True, exist_ok=True)
        with output_path.open("w", encoding="utf-8") as f:
            json.dump(wrapped, f, indent=2, ensure_ascii=False)

        print(f"[OK] {input_path} -> {output_path}")

    except Exception as e:
        print(f"[ERROR] {input_path}: {e}")


def main() -> None:
    if not INPUT_DIR.exists():
        print(f"[ERROR] Input folder not found: {INPUT_DIR}")
        return

    if OUTPUT_DIR.exists():
        shutil.rmtree(OUTPUT_DIR)

    for file_path in INPUT_DIR.rglob("*.json"):
        relative_path = file_path.relative_to(INPUT_DIR)
        output_path = OUTPUT_DIR / relative_path
        process_file(file_path, output_path)

    print(f"\nDone. Converted files written to: {OUTPUT_DIR}")


if __name__ == "__main__":
    main()