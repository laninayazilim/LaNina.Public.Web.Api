Digital Signage ve out-of-home advertising yazılım çözümleri geliştiren İstanbul(Avr.) ekibimize katılarak yazılım geliştirme yaşam döngüsünün her aşamasında sorumluluk alacak, tam zamanlı “Full Stack - Yazılım Geliştirme Uzmanı” arayışımız bulunmaktadır.

Adayın;

- Erkek ise; askerlik görevini tamamlamış veya muaf olması,
- Tercihen mühendislik alanında lisans derecesi olması,
- Teknik dökümanları takip edebilecek seviyede İngilizce bilgisine sahip olması,
- Bir yıl veya üzerinde profesyonel yazılım geliştirme deneyimi olması,
- C#, ASP.NET Core, EF Core, Angular ve MS SQL Server ile proje geliştirme deneyimi olması,
- Tercihen ASP.NET ZERO veya ASP.NET Boilerplate ile proje geliştirme deneyimi olması,
- Tercihen object-oriented programming, SOLID principles, multy tenancy, layered architecture, domain driven design, modular design, test automation gibi konularda fikir sahibi olması

beklenmektedir.

---

Burada kaynak kodu yer alan proje laninayazilim.com üzerinden yayınlanmaktadır. 
Örn: http://laninayazilim.com/api/applicant/HealthCheck

İlana başvurmak isteyen adaylar [ApplicantController.cs](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/master/Controllers/ApplicantController.cs) dosyasını inceleyebilirler. Burada yer alan endpointlere ilgili requestleri gönderip
yönlendirmeleri takip ederek başvuru yapabilir ve süreçlerini ilerletebilirler.

[Apply](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L46) Başvurunuzu göndermek için bu metodu kullanabilirsiniz. Doğru şekilde başvuru yapabilmeniz için kodu incelemeniz beklenmektedir.

[ConfirmEmail](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L135) Başvuru sonrası mailinize EmailConfirmationKey gelecek. Bu metodu kullanarak mail adresinizi doğrulamanızı bekliyoruz.

[GetInterviewDates](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L166) Başvurunuz değerlendirildikten sonra, eğer olumlu değerlendirme yapılırsa, yüzyüze görüşmeye davet edildiğiniz bir mail alacaksınız. Bu mailde yer alan ApplicantKey değerini kullanarak bu metodu kullandığınızda uygun görüşme saatlerinin listesine ulaşacaksınız. 

[SetInterviewDate](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L204) Görüşme saatlerinin listesini incelediniz, size uygun saate karar verdiniz. Bu metodu kullanarak o saati bize de bildirmenizi bekliyoruz ki biz de bu görüşme için hazırlıklarımızı yapalım.

Eğer yüzyüz görüşmemiz de olumlu değerlendirilir ise, çok basit bir deneme projemiz var, bunu hazırlamanızı beklediğimize dair bir mail alacaksınız. Bu mailde gerekli yönlendirmeleri bulacaksınız ayrıca **yeni bir ApplicantKey** değeriniz olacak. Projenizi tamamladıktan sonra bu yeni ApplicantKey değerini kullanarak [GetInterviewDates](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L166) ve [SetInterviewDate](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/b82992ac75c3e6f8898ceb701e0e7c292ac77aef/Controllers/ApplicantController.cs#L204) metodlarını kullanarak ikinci görüşme için randevunuzu ayarlayabilirsiniz.

**Başvurusunu database'de flags tablosunda yer alan herhangi bir key ile yapan adaylar, flag olmadan başvuru yapanlara göre avantajlı olacaklardır.** Flag değeri elde edebilmek için [ApplicantController.cs](https://github.com/laninayazilim/LaNina.Public.Web.Api/blob/master/Controllers/ApplicantController.cs) içinde yer alan diğer endpointleri de dikkatlice incelemeniz gerekebilir.

Endpointleri kullanmak için herhangi bir yöntem kısıtı yoktur.

Bir ip adresinden aynı anda atılabilecek istek sayısı 10 ile limitlidir.

Bir ip adresinden 10sn içerisinde atılabilecek istek sayısı 10 ile limitlidir.

Kullanım esnasında herhangi bir sorunla karşılaşmanız durumunda Issues bölümünü kullanarak bizi bilgilendirebilirsiniz.

