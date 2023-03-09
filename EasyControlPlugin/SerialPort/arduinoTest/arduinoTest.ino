
String rx_data = "";
/////////////////////////////////////////////////////////////////////////////////////////////////////////
String GetStringData(String data, int index)
{
  int count = data.length();
  if (count > 5 && data[0] == '>' && data[1] == '>' && data[count - 1] == '<' && data[count - 2] == '<')
  {
    int currentIndex = 0;
    int start = 2;
    int end = -1;
    for (int i = 2; i < data.length(); i++)
    {
      if (data[i] == ',')
      {
        end = i;
        currentIndex++;
        if (currentIndex == index)
        {
          return data.substring(start, end);
        }
        else
        {
          start = i + 1;
        }
      }
    }
    return "index Error!";
  }
  else
  {
    return "not Exist!";
  }
}
int GetIntData(String data, int index)
{
  String temp = GetStringData(data, index);
  int x = temp.toInt();
  return x;
}
float GetFloatData(String data, int index)
{
  String temp = GetStringData(data, index);
  float x = temp.toFloat();
  return x;
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////
void setup()
{
  // put your setup code here, to run once:
  Serial.begin(115200);
}

void loop()
{
  //Serial.println("-");
  // put your main code here, to run repeatedly:
  while (Serial.available() > 0)
  {
    rx_data += char(Serial.read());
  }
  int count = rx_data.length();
  if (count > 0)
  {
    Serial.println(rx_data);
    int test1 = GetIntData(rx_data, 1);
    Serial.println(test1 + 10000);
    float test2 = GetFloatData(rx_data, 2);
    Serial.println(test2 + 10000);
    String test3 = GetStringData(rx_data, 3);
    Serial.println(test3);
    rx_data = "";
  }
}
