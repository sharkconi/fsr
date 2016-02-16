#!/usr/bin/env python
# -*- coding: utf-8 -*-

from flask import Flask, render_template, redirect, url_for
import MySQLdb
from decimal import *

app = Flask(__name__)

def handle_ribao_data(res):
    ds = []
    all_in = Decimal(0.0)
    for line in res:
        if Decimal(line[5]) == 0.0 or Decimal(line[6]) == 0.0:
            continue
        d = (line[2], '{:10,.2f}'.format(Decimal(line[6])), '{:10,.2f}'.format(Decimal(line[5])), '{:10,.2f}'.format(Decimal(line[6])/Decimal(line[5])))
        all_in = all_in + Decimal(line[6])
        ds.append(d)
    return (ds, all_in)

@app.route('/m')
def fsr_mobile_page():
    conn = MySQLdb.connect(
        host = 'rds3vt9mpg9qj5biy748a.mysql.rds.aliyuncs.com',
        port = 3306,
        user = 'fsradmin',
        passwd = 'fsradmin',
        db = 'fsr',
        charset = 'utf8',
    )
    cur = conn.cursor()
    
    result = cur.execute("select * from ribao where date = \'2015-12-03\'")
    ds, all_in = handle_ribao_data(cur.fetchmany(result))
    cur.close()
    conn.close()
 
    return render_template('fsr_mobile.html', ds = ds, d="2015-12-03", all_in='{:6,.2f}'.format(all_in/10000))

@app.route('/web')
def fsr_web_page():
    conn = MySQLdb.connect(
        host = 'rds3vt9mpg9qj5biy748a.mysql.rds.aliyuncs.com',
        port = 3306,
        user = 'fsradmin',
        passwd = 'fsradmin',
        db = 'fsr',
        charset = 'utf8',
        connect_timeout = 5,
    )
    cur = conn.cursor()
    
    result = cur.execute("select * from ribao where date = \'2015-12-03\'")
    ds = cur.fetchmany(result)
    cur.close()
    conn.close()
 
    return render_template('fsr_web.html', ds = ds)

@app.route("/")
def fsr_main():
    return redirect(url_for('fsr_web_page'))

if __name__ == "__main__":
    app.run(host = '0.0.0.0', debug = True)
