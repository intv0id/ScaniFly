# ScaniFly

ScaniFly is an open-source, AI-powered desktop application designed to automatically classify, rename, and organize your scanned PDF documents locally.

## Features
- **Background Folder Monitoring**: Automatically detects new PDFs in your configured `Monitored Directory`.
- **Local AI Semantics**: Uses LLMs running locally via Ollama to "read" the PDF and suggest a smart filename and a destination subfolder based on the context.
- **OCR Options**: Choose how to extract text from your PDFs:
  - `Vision LLM`: Renders pages to images and uses a vision-capable LLM (e.g., LLaVA).
  - `Tesseract OCR`: Uses traditional OCR then a standard text LLM (Fallback).
  - `Native OS OCR`: Uses Windows Media OCR or Apple Vision frameworks (Fallback).
- **Proposals Review**: Review the AI's suggestions side-by-side with a PDF preview before accepting or editing them.
- **Completely Local & Private**: No cloud accounts, no API keys, no data leaving your machine.
- **Auto-Updater**: Seamlessly downloads new versions from GitHub Releases.

## Requirements
- **Ollama**: ScaniFly relies entirely on local models. You must install [Ollama](https://ollama.com/) and download a model (like `llama3` or `llava`) for the app to function correctly.

## Installation
Visit the [GitHub Releases](https://github.com/YOUR_ORG/ScaniFly/releases/latest) page to download the latest installer for Windows (`.exe`) or macOS (`.dmg`).

## Screenshots
<img src="https://raw.githubusercontent.com/intv0id/ScaniFly/0939c805e981a6f714176dcf1e9abab27ac3b602/docs/settings.png" width="400"/>
<img src="https://raw.githubusercontent.com/intv0id/ScaniFly/0939c805e981a6f714176dcf1e9abab27ac3b602/docs/proposals.png" width="400"/>

## License
MIT
