#!/bin/sh

run() {
    # Run the C# program
    cd ~/Downloads/Programing/Moogle-main/MoogleServer
    dotnet watch run
}

report() {
    # Create a PDF report from a .tex file
    pdflatex Informe.tex
}

slides() {
    pdflatex Presentacion.tex
}

showSlides() {
    if [ -f Presentacion.pdf ]; then
        open Presentacion.pdf
    else
        pdflatex Presentacion.tex
        open Presentacion.pdf
    fi

}

showReport() {
    # Show the report.pdf using a PDF reader, or create it from the .tex file if it doesn't exist
    if [ -f Informe.pdf ]; then
        # The report.pdf file already exists, so open it with a PDF reader
        open Informe.pdf
    else
        # The report.pdf file doesn't exist, so create it from the .tex file
        pdflatex Informe.tex
        open Informe.pdf
    fi
}

clean() {
    # Delete all auxiliary files created during the compilation of the .tex file
    rm -f Informe.fdb_latexmk Informe.fls Informe.synctex.gz Informe.aux Informe.log Informe.out Informe.toc Presentacion.aux Presentacion.log Presentacion.out Presentacion.toc Presentacion.fls Presentacion.nav Presentacion.snm Presentacion.fdb_latexmk
}

# Check the input option and execute the corresponding function
case "$1" in
    run)
        run
        ;;
    report)
        report
        ;;
    slides)
        slides
        ;;
    showSlides)
        showSlides;;
    showReport)
        showReport
        ;;
    clean)
        clean
        ;;
    *)
        echo "Invalid option. Usage: ./script.sh [run|report|slides|showSlides|showReport|clean]"
        exit 1
        ;;
esac



