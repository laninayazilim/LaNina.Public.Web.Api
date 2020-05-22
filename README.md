| :mega: Duyurular |
|--------------|
| 2020-02-20: İşe alım sürecimiz sona ermiştir. Ancak denemek isteyenler için uygulama yayında kalmaya devam edecektir.|


 
---

Yurt dışındaki müşterimize özel digital signage ve out-of-home advertising yazılım çözümleri geliştiren **İstanbul(Avr.) ekibimize** katılarak yazılım geliştirme yaşam döngüsünün her aşamasında sorumluluk alacak, tam zamanlı **“Full Stack - Yazılım Geliştirme Uzmanı”** arayışımız bulunmaktadır.

Adayın;

- Erkek ise; askerlik görevini tamamlamış veya muaf olması,
- *Tercihen* mühendislik alanında lisans derecesi olması,
- Teknik dökümanları takip edebilecek seviyede İngilizce bilgisine sahip olması,
- Bir-üç yıl profesyonel yazılım geliştirme deneyimi olması,
- C#, ASP.NET Core, EF Core, Angular ve MS SQL Server ile proje geliştirme deneyimi olması,
- *Tercihen* [ASP.NET Zero](https://aspnetzero.com/) veya [ASP.NET Boilerplate](https://aspnetboilerplate.com) ile proje geliştirme deneyimi olması,
- *Tercihen* object-oriented programming, SOLID principles, multi-tenancy, layered architecture, domain driven design, modular design, test automation gibi konularda fikir sahibi olması

beklenmektedir.

---

Medium makalemize göz atın >>> [Farklı bir yöntem deneyelim istedik...](http://bit.ly/2L3v3K5)

---

Burada kaynak kodu yer alan proje laninayazilim.com üzerinden yayınlanmaktadır. 
http://laninayazilim.com/api/applicant/HealthCheck

İlana başvurmak isteyen adaylar [ApplicantController.cs](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/master/Controllers/ApplicantController.cs) dosyasını inceleyebilirler. Burada yer alan endpointlere ilgili requestleri gönderip
yönlendirmeleri takip ederek başvuru yapabilir ve süreçlerini ilerletebilirler.

### [Apply](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L46) 
Başvurunuzu göndermek için bu metodu kullanabilirsiniz. Doğru şekilde başvuru yapabilmeniz için kodu incelemeniz beklenmektedir.

**Başvuruyu database'de flags tablosunda yer alan herhangi bir key ile yapmak tercih nedeni olacaktır. Flag değeri ele geçirebilmek için [ApplicantController.cs](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/master/Controllers/ApplicantController.cs) içinde yer alan diğer endpointleri de dikkatlice incelemeniz gerekebilir.**

### [ConfirmEmail](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L135) 
Başvuru sonrası mailinize EmailConfirmationKey gelecek. Bu metodu kullanarak mail adresinizi doğrulamanızı bekliyoruz.

### [GetInterviewDates](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L166) 
Başvurunuz değerlendirildikten sonra, eğer olumlu değerlendirme yapılırsa, yüzyüze görüşmeye davet edildiğiniz bir mail alacaksınız. Bu mailde yer alan ApplicantKey değerini kullanarak bu metodu kullandığınızda uygun görüşme saatlerinin listesine ulaşacaksınız. 

### [SetInterviewDate](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L204) 
Görüşme saatlerinin listesini incelediniz, size uygun saate karar verdiniz. Bu metodu kullanarak o saati bize de bildirmenizi bekliyoruz ki biz de bu görüşme için hazırlıklarımızı yapalım.

---

Endpointleri kullanmak için herhangi bir yöntem kısıtı yoktur.

Bir ip adresinden aynı anda atılabilecek istek sayısı 10 ile limitlidir.

Bir ip adresinden 10sn içerisinde atılabilecek istek sayısı 10 ile limitlidir.

Request boyutu 5MB ile limitlidir.

Kullanım esnasında herhangi bir sorunla karşılaşmanız durumunda [Issues](https://github.com/laninayazilim/LaNina.Public.Web.Api/issues) bölümünü kullanarak bizi bilgilendirebilirsiniz.

