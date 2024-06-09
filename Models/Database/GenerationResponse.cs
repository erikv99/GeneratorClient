    public class GenerationResponse
    {
        public int? Id { get; set; }
        public required DateTime CreatedOn { get; set; }
        public required bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public int? ImageId { get; set; }
        public virtual Image? Image { get; set; }

        public int RequestId { get; set; }
        public virtual GenerationRequest Request { get; set; } = null!;
    }