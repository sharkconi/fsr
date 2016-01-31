#!/usr/bin/env python
# -*- coding: utf-8 -*-

from flask import Flask, render_template, redirect, url_for
import MySQLdb

app = Flask(__name__)

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
    ds = cur.fetchmany(result)
    cur.close()
    conn.close()
 
    return render_template('fsr_mobile.html', ds = ds)

@app.route('/web')
def fsr_web_page():
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
    ds = cur.fetchmany(result)
    cur.close()
    conn.close()
 
    return render_template('fsr_web.html', ds = ds)

@app.route("/")
def fsr_main():
    return redirect(url_for('fsr_web_page'))

if __name__ == "__main__":
    app.run(host = '0.0.0.0', debug = True)