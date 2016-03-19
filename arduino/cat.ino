#include <Servo.h>
Servo segment_s_s;
Servo roata_s_s;
Servo segment_s_d;
Servo roata_s_d;
Servo segment_f_s;
Servo roata_f_s;
Servo segment_f_d;
Servo roata_f_d;
int stare=0;

void spate_stg()
{
  segment_s_s.write(100);
  roata_s_s.write(100);
  delay(250);
  roata_s_s.write(60);
  delay(150);
  segment_s_s.write(70);
  delay(150);
}

void spate_dr()
{
  segment_s_d.write(25);
  roata_s_d.write(25);
  delay(250);
  roata_s_d.write(65);
  delay(150);
  segment_s_d.write(55);
  delay(150);
}

void fata_stg()
{
  segment_f_s.write(30);
  roata_f_s.write(120);
  delay(250);
  roata_f_s.write(25);
  segment_f_s.write(50);
  delay(250);
  segment_f_s.write(70);
  roata_f_s.write(100);
  delay(150);
}

void fata_dr()
{
  segment_f_d.write(95);
  roata_f_d.write(45);
  delay(250);
  roata_f_d.write(135);
  segment_f_d.write(80);
  delay(250);
  segment_f_d.write(55);
  roata_f_d.write(55);
  delay(150);
}
void setup()
{
  segment_s_s.attach(7);
  roata_s_s.attach(11);
  segment_s_d.attach(4);
  roata_s_d.attach(12);
  segment_f_s.attach(3);
  roata_f_s.attach(8);
  segment_f_d.attach(9);
  roata_f_d.attach(10);
  Serial.begin(9600);
  while (!Serial) {}
  stai();
}

void stai(){
  segment_f_s.write(30);
  roata_f_s.write(120);
  segment_s_s.write(100);
  roata_s_s.write(100);
  segment_f_d.write(95);
  roata_f_d.write(45);
  segment_s_d.write(25);
  roata_s_d.write(25);
}

void mers(){
  segment_f_s.write(30);
  roata_f_s.write(120);
  segment_s_s.write(100);
  roata_s_s.write(100);
  delay(300);
  roata_f_s.write(25);
  segment_f_s.write(50);
  roata_s_s.write(60);
  delay(300);
  segment_f_s.write(70);
  roata_f_s.write(100);
  segment_s_s.write(70);
  delay(150);
  //////////////////////////////////////////
  segment_f_d.write(95);
  roata_f_d.write(45);
  segment_s_d.write(25);
  roata_s_d.write(25);
  delay(300);
  roata_f_d.write(135);
  segment_f_d.write(80);
  
  roata_s_d.write(65);
  delay(300);
  segment_f_d.write(55);
  roata_f_d.write(55);
  segment_s_d.write(55);
  delay(150);
}
void loop(){
//spate_stg();
//fata_dr();
//spate_dr();
//fata_stg();
//
char op;
if (Serial.available()) {
    op=Serial.read();
    if (op=='w'){stare=1;}
    if (op=='s'){stare=0;}
    Serial.write(op);
}
if (stare) {mers();}
else {stai();}


}