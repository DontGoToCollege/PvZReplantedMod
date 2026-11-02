"""Turns a REA il2cpp evidence bundle into searchable C#-like dumps.

Usage: python make_dump.py <il2cpp-bundle.json> <output folder>
Writes dump-game.cs (Assembly-CSharp only), dump-all.cs and strings.txt.
The output is the game's own names, so it stays out of git (see .gitignore).
"""
import json
import os
import sys
from collections import defaultdict


def full_name(types, i):
    t = types[i]
    if t["declaring_type"] >= 0:
        return full_name(types, t["declaring_type"]) + "+" + t["name"]
    return (t["namespace"] + "." if t["namespace"] else "") + t["name"]


def render_type(m, i, fields, methods, props, params):
    out = [f"class {full_name(m['types'], i)}  // type {i}\n{{"]
    for name, ftype in fields[i]:
        out.append(f"    {ftype} {name};")
    for name in props[i]:
        out.append(f"    property {name};")
    for j in methods[i]:
        addr = m["method_addresses"][j]
        rva = addr["rva"] if addr["state"] == "code" else addr["state"]
        sig = ", ".join(f"{t} {n}" for n, t in params[j])
        out.append(f"    {m['return_types'][j]['name']} {m['methods'][j]['name']}({sig});  // {rva}")
    out.append("}\n")
    return "\n".join(out)


def main(bundle_path, out_dir):
    with open(bundle_path, encoding="utf-8") as f:
        bundle = json.load(f)
    m = next(r["normalized_result"] for r in bundle["records"] if r["operation"] == "inspect_il2cpp_metadata")

    fields, methods, props, params = defaultdict(list), defaultdict(list), defaultdict(list), defaultdict(list)
    for f, t in zip(m["fields"], m["field_types"]):
        fields[f["declaring_type"]].append((f["name"], t["name"]))
    for p in m["properties"]:
        props[p["declaring_type"]].append(p["name"])
    for j, x in enumerate(m["methods"]):
        methods[x["declaring_type"]].append(j)
    for p, t in zip(m["parameters"], m["parameter_types"]):
        params[p["method"]].append((p["name"], t["name"]))

    game = next(i for i in m["images"] if i["name"] == "Assembly-CSharp.dll")
    game_range = range(game["type_start"], game["type_start"] + game["type_count"])
    assert len(game_range) > 0 and len(m["fields"]) == len(m["field_types"])

    os.makedirs(out_dir, exist_ok=True)
    with open(os.path.join(out_dir, "dump-game.cs"), "w", encoding="utf-8") as f:
        for i in game_range:
            f.write(render_type(m, i, fields, methods, props, params) + "\n")
    with open(os.path.join(out_dir, "dump-all.cs"), "w", encoding="utf-8") as f:
        for image in m["images"]:
            f.write(f"// ===== {image['name']} =====\n")
            for i in range(image["type_start"], image["type_start"] + image["type_count"]):
                f.write(render_type(m, i, fields, methods, props, params) + "\n")
    with open(os.path.join(out_dir, "strings.txt"), "w", encoding="utf-8") as f:
        for s in m["string_literals"]:
            f.write(repr(s["value"]) + "\n")


if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2])
