#!/usr/bin/env nix-shell
#! nix-shell -i bash
#! nix-shell -p ffmpeg findutils coreutils

output="/drives/500GSSD/merged.mp4"

# Find .mp4 files modified in the last 12 hours (720 minutes), sort oldest first
# Using null delimiters to safely handle spaces in filenames
mapfile -d '' files < <(find . -maxdepth 1 -type f -name "*.mp4" -mmin -720 -printf "%T@ %p\0" | sort -zn | awk -v RS='\0' '{sub(/^[^ ]+ /,""); print $0}' | tr '\n' '\0')

if [ ${#files[@]} -eq 0 ]; then
  echo "No .mp4 files found in the last 12 hours."
  exit 1
fi

# Create a temporary file list for ffmpeg
tmpfile=$(mktemp)
for f in "${files[@]}"; do
  # Escape single quotes in filenames
  safe_f=$(printf "%q" "$f")
  echo "file '$PWD/$f'" >> "$tmpfile"
done

# Merge using ffmpeg
ffmpeg -f concat -safe 0 -i "$tmpfile" -c copy "$output"

rm "$tmpfile"

echo "Merged ${#files[@]} files into $output"
