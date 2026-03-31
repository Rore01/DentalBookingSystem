using FluentEmail.Core;

namespace DentalBookingSystemApi.Common;

public class EmailService(IFluentEmail fluentEmail)
{
    public async Task SendBookingConfirmationAsync(
        Patient patient,
        Booking booking,
        Treatment treatment)
    {
        var date = booking.Date.ToDateTime(TimeOnly.MinValue)
            .ToString("dddd, MMMM d yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        var startTime = booking.StartTime.ToString("HH:mm");
        var endTime = booking.EndTime.ToString("HH:mm");
        var price = treatment.Price.ToString("N0");
        var duration = treatment.DurationMinutes.ToString();
        var patientName = patient.FirstName;
        var treatmentName = treatment.Name;

        var html = $"""
<!DOCTYPE html>
<html lang="en">
<head><meta charset="UTF-8"/><meta name="viewport" content="width=device-width,initial-scale=1.0"/></head>
<body style="margin:0;padding:0;background:#f4f7f6;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f7f6;padding:40px 0;">
    <tr><td align="center">
      <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;">
        <tr><td style="background:#0f2027;border-radius:16px 16px 0 0;padding:32px 40px;text-align:center;">
          <div style="font-size:22px;font-weight:700;color:white;letter-spacing:1px;">🦷 DentaCare</div>
          <div style="font-size:13px;color:rgba(255,255,255,0.5);margin-top:4px;">Your dental clinic online</div>
        </td></tr>
        <tr><td style="background:#1a7a6e;padding:24px 40px;text-align:center;">
          <div style="font-size:36px;margin-bottom:8px;">✅</div>
          <div style="font-size:22px;font-weight:700;color:white;">Booking Confirmed!</div>
          <div style="font-size:14px;color:rgba(255,255,255,0.8);margin-top:6px;">Your appointment has been successfully booked.</div>
        </td></tr>
        <tr><td style="background:white;padding:40px;">
          <p style="font-size:15px;color:#2c4a52;margin:0 0 24px;">Hi <strong>{patientName}</strong>,</p>
          <p style="font-size:14px;color:#555;margin:0 0 28px;line-height:1.6;">Thank you for booking with DentaCare. Here are your appointment details:</p>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f8fffe;border:1px solid #d0ede8;border-radius:12px;overflow:hidden;margin-bottom:28px;">
            <tr style="background:#e8f5f0;"><td colspan="2" style="padding:14px 20px;font-size:12px;font-weight:700;color:#1a7a6e;letter-spacing:1px;">APPOINTMENT DETAILS</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;width:40%;border-bottom:1px solid #e8f5f0;">Treatment</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{treatmentName}</td></tr>
            <tr style="background:#fafefe;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">Date</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{date}</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">Time</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{startTime} – {endTime}</td></tr>
            <tr style="background:#fafefe;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">Duration</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{duration} minutes</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;">Price</td><td style="padding:14px 20px;font-size:16px;font-weight:700;color:#1a7a6e;">{price} kr</td></tr>
          </table>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#fff8f0;border:1px solid #f5d9b0;border-radius:10px;margin-bottom:28px;">
            <tr><td style="padding:16px 20px;font-size:13px;color:#8a6030;line-height:1.6;">⏰ <strong>Reminder:</strong> You will receive a reminder email 24 hours before your appointment. If you need to cancel or reschedule, please log in to your account.</td></tr>
          </table>
          <p style="font-size:14px;color:#555;line-height:1.6;margin:0;">We look forward to seeing you!<br/><strong style="color:#0f2027;">The DentaCare Team</strong></p>
        </td></tr>
        <tr><td style="background:#f4f7f6;border-radius:0 0 16px 16px;padding:24px 40px;text-align:center;border-top:1px solid #e0e8ec;">
          <div style="font-size:12px;color:#aab5bc;">© DentaCare · This is an automated message, please do not reply.</div>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>
""";

        await fluentEmail
            .To(patient.Email, $"{patient.FirstName} {patient.LastName}")
            .Subject("Booking Confirmed — DentaCare")
            .Body(html, isHtml: true)
            .SendAsync();
    }

    public async Task SendReminderAsync(
        Patient patient,
        Booking booking,
        Treatment treatment)
    {
        var date = booking.Date.ToDateTime(TimeOnly.MinValue)
            .ToString("dddd, MMMM d yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        var startTime = booking.StartTime.ToString("HH:mm");
        var endTime = booking.EndTime.ToString("HH:mm");
        var price = treatment.Price.ToString("N0");
        var patientName = patient.FirstName;
        var treatmentName = treatment.Name;

        var html = $"""
<!DOCTYPE html>
<html lang="en">
<head><meta charset="UTF-8"/><meta name="viewport" content="width=device-width,initial-scale=1.0"/></head>
<body style="margin:0;padding:0;background:#f4f7f6;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f7f6;padding:40px 0;">
    <tr><td align="center">
      <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;">
        <tr><td style="background:#0f2027;border-radius:16px 16px 0 0;padding:32px 40px;text-align:center;">
          <div style="font-size:22px;font-weight:700;color:white;letter-spacing:1px;">🦷 DentaCare</div>
          <div style="font-size:13px;color:rgba(255,255,255,0.5);margin-top:4px;">Your dental clinic online</div>
        </td></tr>
        <tr><td style="background:#c9a96e;padding:24px 40px;text-align:center;">
          <div style="font-size:36px;margin-bottom:8px;">⏰</div>
          <div style="font-size:22px;font-weight:700;color:white;">Appointment Tomorrow!</div>
          <div style="font-size:14px;color:rgba(255,255,255,0.85);margin-top:6px;">This is your 24-hour reminder.</div>
        </td></tr>
        <tr><td style="background:white;padding:40px;">
          <p style="font-size:15px;color:#2c4a52;margin:0 0 24px;">Hi <strong>{patientName}</strong>,</p>
          <p style="font-size:14px;color:#555;margin:0 0 28px;line-height:1.6;">Just a friendly reminder that you have a dental appointment with DentaCare <strong>tomorrow</strong>. Here are your details:</p>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f8fffe;border:1px solid #d0ede8;border-radius:12px;overflow:hidden;margin-bottom:28px;">
            <tr style="background:#e8f5f0;"><td colspan="2" style="padding:14px 20px;font-size:12px;font-weight:700;color:#1a7a6e;letter-spacing:1px;">YOUR APPOINTMENT</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;width:40%;border-bottom:1px solid #e8f5f0;">Treatment</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{treatmentName}</td></tr>
            <tr style="background:#fafefe;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">Date</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{date}</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">Time</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{startTime} – {endTime}</td></tr>
            <tr style="background:#fafefe;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;">Price</td><td style="padding:14px 20px;font-size:16px;font-weight:700;color:#1a7a6e;">{price} kr</td></tr>
          </table>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f8fffe;border:1px solid #d0ede8;border-radius:10px;margin-bottom:28px;">
            <tr><td style="padding:16px 20px;">
              <div style="font-size:12px;font-weight:700;color:#1a7a6e;letter-spacing:1px;margin-bottom:10px;">BEFORE YOUR VISIT</div>
              <div style="font-size:13px;color:#555;line-height:1.8;">🪥 Brush and floss before your appointment<br/>🕐 Arrive 5 minutes early<br/>💳 Bring your insurance card if applicable<br/>📵 If you need to cancel, please do so at least 24h in advance</div>
            </td></tr>
          </table>
          <p style="font-size:14px;color:#555;line-height:1.6;margin:0;">See you tomorrow!<br/><strong style="color:#0f2027;">The DentaCare Team</strong></p>
        </td></tr>
        <tr><td style="background:#f4f7f6;border-radius:0 0 16px 16px;padding:24px 40px;text-align:center;border-top:1px solid #e0e8ec;">
          <div style="font-size:12px;color:#aab5bc;">© DentaCare · This is an automated message, please do not reply.</div>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>
""";

        await fluentEmail
            .To(patient.Email, $"{patient.FirstName} {patient.LastName}")
            .Subject("Reminder: Your Appointment Tomorrow — DentaCare")
            .Body(html, isHtml: true)
            .SendAsync();
    }
    public async Task SendRescheduleConfirmationAsync(
    Patient patient,
    Booking booking,
    Treatment treatment)
    {
        var date = booking.Date.ToDateTime(TimeOnly.MinValue)
            .ToString("dddd, MMMM d yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        var startTime = booking.StartTime.ToString("HH:mm");
        var endTime = booking.EndTime.ToString("HH:mm");
        var price = treatment.Price.ToString("N0");
        var patientName = patient.FirstName;
        var treatmentName = treatment.Name;

        var html = $"""
<!DOCTYPE html>
<html lang="en">
<head><meta charset="UTF-8"/><meta name="viewport" content="width=device-width,initial-scale=1.0"/></head>
<body style="margin:0;padding:0;background:#f4f7f6;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f7f6;padding:40px 0;">
    <tr><td align="center">
      <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;">
        <tr><td style="background:#0f2027;border-radius:16px 16px 0 0;padding:32px 40px;text-align:center;">
          <div style="font-size:22px;font-weight:700;color:white;letter-spacing:1px;">🦷 DentaCare</div>
          <div style="font-size:13px;color:rgba(255,255,255,0.5);margin-top:4px;">Your dental clinic online</div>
        </td></tr>
        <tr><td style="background:#2aa896;padding:24px 40px;text-align:center;">
          <div style="font-size:36px;margin-bottom:8px;">🔄</div>
          <div style="font-size:22px;font-weight:700;color:white;">Appointment Rescheduled!</div>
          <div style="font-size:14px;color:rgba(255,255,255,0.8);margin-top:6px;">Your appointment has been moved to a new time.</div>
        </td></tr>
        <tr><td style="background:white;padding:40px;">
          <p style="font-size:15px;color:#2c4a52;margin:0 0 24px;">Hi <strong>{patientName}</strong>,</p>
          <p style="font-size:14px;color:#555;margin:0 0 28px;line-height:1.6;">Your appointment has been successfully rescheduled. Here are your updated details:</p>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f8fffe;border:1px solid #d0ede8;border-radius:12px;overflow:hidden;margin-bottom:28px;">
            <tr style="background:#e8f5f0;"><td colspan="2" style="padding:14px 20px;font-size:12px;font-weight:700;color:#1a7a6e;letter-spacing:1px;">NEW APPOINTMENT DETAILS</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;width:40%;border-bottom:1px solid #e8f5f0;">Treatment</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{treatmentName}</td></tr>
            <tr style="background:#fafefe;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">New Date</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{date}</td></tr>
            <tr><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #e8f5f0;">New Time</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #e8f5f0;">{startTime} – {endTime}</td></tr>
            <tr style="background:#fafefe;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;">Price</td><td style="padding:14px 20px;font-size:16px;font-weight:700;color:#1a7a6e;">{price} kr</td></tr>
          </table>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#fff8f0;border:1px solid #f5d9b0;border-radius:10px;margin-bottom:28px;">
            <tr><td style="padding:16px 20px;font-size:13px;color:#8a6030;line-height:1.6;">⏰ <strong>Note:</strong> Your reminder email has been reset and you will receive a new reminder 24 hours before your updated appointment.</td></tr>
          </table>
          <p style="font-size:14px;color:#555;line-height:1.6;margin:0;">We look forward to seeing you!<br/><strong style="color:#0f2027;">The DentaCare Team</strong></p>
        </td></tr>
        <tr><td style="background:#f4f7f6;border-radius:0 0 16px 16px;padding:24px 40px;text-align:center;border-top:1px solid #e0e8ec;">
          <div style="font-size:12px;color:#aab5bc;">© DentaCare · This is an automated message, please do not reply.</div>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>
""";

        await fluentEmail
            .To(patient.Email, $"{patient.FirstName} {patient.LastName}")
            .Subject("Appointment Rescheduled — DentaCare")
            .Body(html, isHtml: true)
            .SendAsync();
    }
    public async Task SendCancellationAsync(
    Patient patient,
    Booking booking,
    Treatment treatment)
    {
        var date = booking.Date.ToDateTime(TimeOnly.MinValue)
            .ToString("dddd, MMMM d yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        var startTime = booking.StartTime.ToString("HH:mm");
        var endTime = booking.EndTime.ToString("HH:mm");
        var price = treatment.Price.ToString("N0");
        var patientName = patient.FirstName;
        var treatmentName = treatment.Name;

        var html = $"""
<!DOCTYPE html>
<html lang="en">
<head><meta charset="UTF-8"/><meta name="viewport" content="width=device-width,initial-scale=1.0"/></head>
<body style="margin:0;padding:0;background:#f4f7f6;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f7f6;padding:40px 0;">
    <tr><td align="center">
      <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;">
        <tr><td style="background:#0f2027;border-radius:16px 16px 0 0;padding:32px 40px;text-align:center;">
          <div style="font-size:22px;font-weight:700;color:white;letter-spacing:1px;">🦷 DentaCare</div>
          <div style="font-size:13px;color:rgba(255,255,255,0.5);margin-top:4px;">Your dental clinic online</div>
        </td></tr>
        <tr><td style="background:#c0392b;padding:24px 40px;text-align:center;">
          <div style="font-size:36px;margin-bottom:8px;">❌</div>
          <div style="font-size:22px;font-weight:700;color:white;">Appointment Cancelled</div>
          <div style="font-size:14px;color:rgba(255,255,255,0.8);margin-top:6px;">Your appointment has been cancelled.</div>
        </td></tr>
        <tr><td style="background:white;padding:40px;">
          <p style="font-size:15px;color:#2c4a52;margin:0 0 24px;">Hi <strong>{patientName}</strong>,</p>
          <p style="font-size:14px;color:#555;margin:0 0 28px;line-height:1.6;">Your appointment has been successfully cancelled. Here are the details of the cancelled booking:</p>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#fde8e8;border:1px solid #f5b8b8;border-radius:12px;overflow:hidden;margin-bottom:28px;">
            <tr style="background:#fde8e8;"><td colspan="2" style="padding:14px 20px;font-size:12px;font-weight:700;color:#c0392b;letter-spacing:1px;">CANCELLED APPOINTMENT</td></tr>
            <tr style="background:white;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;width:40%;border-bottom:1px solid #fde8e8;">Treatment</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #fde8e8;">{treatmentName}</td></tr>
            <tr style="background:#fafafa;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #fde8e8;">Date</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #fde8e8;">{date}</td></tr>
            <tr style="background:white;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;border-bottom:1px solid #fde8e8;">Time</td><td style="padding:14px 20px;font-size:14px;font-weight:600;color:#0f2027;border-bottom:1px solid #fde8e8;">{startTime} – {endTime}</td></tr>
            <tr style="background:#fafafa;"><td style="padding:14px 20px;font-size:13px;color:#8a9aa5;">Price</td><td style="padding:14px 20px;font-size:16px;font-weight:700;color:#c0392b;">{price} kr</td></tr>
          </table>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f8fffe;border:1px solid #d0ede8;border-radius:10px;margin-bottom:28px;">
            <tr><td style="padding:16px 20px;font-size:13px;color:#1a7a6e;line-height:1.6;">💚 <strong>Want to rebook?</strong> Log in to your account and book a new appointment at any time.</td></tr>
          </table>
          <p style="font-size:14px;color:#555;line-height:1.6;margin:0;">We hope to see you soon!<br/><strong style="color:#0f2027;">The DentaCare Team</strong></p>
        </td></tr>
        <tr><td style="background:#f4f7f6;border-radius:0 0 16px 16px;padding:24px 40px;text-align:center;border-top:1px solid #e0e8ec;">
          <div style="font-size:12px;color:#aab5bc;">© DentaCare · This is an automated message, please do not reply.</div>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>
""";

        await fluentEmail
            .To(patient.Email, $"{patient.FirstName} {patient.LastName}")
            .Subject("Appointment Cancelled — DentaCare")
            .Body(html, isHtml: true)
            .SendAsync();
    }
}