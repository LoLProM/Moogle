#!/bin/sh

run() {
    cd ~/Downloads/Programing/Moogle-main/MoogleServer
    dotnet watch run
}

report() {
    pdflatex Informe.tex
}

slides() {
    pdflatex Presentacion.tex
}

showSlides() {
    if ! [ -f Presentacion.pdf ]; then
        pdflatex Presentacion.tex
    fi

    open Presentacion.pdf

}

showReport() {
    if [ -f Informe.pdf ]; then
        open Informe.pdf
    else
        pdflatex Informe.tex
        open Informe.pdf
    fi
}

clean() {
    rm -f Informe.fdb_latexmk Informe.fls Informe.synctex.gz Informe.aux Informe.log Informe.out Informe.toc Presentacion.aux Presentacion.log Presentacion.out Presentacion.toc Presentacion.fls Presentacion.nav Presentacion.snm Presentacion.fdb_latexmk
}

case $1 in
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
        showSlides $2
        ;;
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



